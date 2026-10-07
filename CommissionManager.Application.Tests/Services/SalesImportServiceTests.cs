using CommissionManager.Application.Dtos;
using CommissionManager.Application.Errors;
using CommissionManager.Application.Services;
using CommissionManager.Application.Tests.Fakes;
using CommissionManager.Domain.Errors;

namespace CommissionManager.Application.Tests.Services;

public class SalesImportServiceTests
{
    private static readonly SaleInputDto[] ChallengeSales =
    [
        new("João Silva", 1200.50m), new("João Silva", 950.75m), new("João Silva", 1800.00m),
        new("João Silva", 1400.30m), new("João Silva", 1100.90m), new("João Silva", 1550.00m),
        new("João Silva", 1700.80m), new("João Silva", 250.30m), new("João Silva", 480.75m),
        new("João Silva", 320.40m),
        new("Maria Souza", 2100.40m), new("Maria Souza", 1350.60m), new("Maria Souza", 950.20m),
        new("Maria Souza", 1600.75m), new("Maria Souza", 1750.00m), new("Maria Souza", 1450.90m),
        new("Maria Souza", 400.50m), new("Maria Souza", 180.20m), new("Maria Souza", 90.75m),
        new("Carlos Oliveira", 800.50m), new("Carlos Oliveira", 1200.00m), new("Carlos Oliveira", 1950.30m),
        new("Carlos Oliveira", 1750.80m), new("Carlos Oliveira", 1300.60m), new("Carlos Oliveira", 300.40m),
        new("Carlos Oliveira", 500.00m), new("Carlos Oliveira", 125.75m),
        new("Ana Lima", 1000.00m), new("Ana Lima", 1100.50m), new("Ana Lima", 1250.75m),
        new("Ana Lima", 1400.20m), new("Ana Lima", 1550.90m), new("Ana Lima", 1650.00m),
        new("Ana Lima", 75.30m), new("Ana Lima", 420.90m), new("Ana Lima", 315.40m),
    ];

    private readonly FakeSalesWriter _writer = new();

    private SalesImportService NewService(FakeSalesParser parser) => new(parser, _writer);

    private static MemoryStream NewContent() => new([1, 2, 3]);

    [Fact]
    public async Task ImportAsync_WithChallengeData_PersistsSellersAndReturnsSummary()
    {
        using var content = NewContent();

        var result = await NewService(FakeSalesParser.Returning(ChallengeSales)).ImportAsync(content);

        Assert.True(result.IsSuccess);
        Assert.Equal(new SalesImportSummaryDto(4, 36, 37321.30m), result.Value);
        Assert.Equal(1, _writer.Calls);
        Assert.Equal(4, _writer.ReceivedSellers.Count);
        Assert.Equal(36, _writer.ReceivedSellers.Sum(s => s.Sales.Count));
    }

    [Fact]
    public async Task ImportAsync_GroupsNamesIgnoringCaseAndSurroundingSpaces()
    {
        using var content = NewContent();
        var parser = FakeSalesParser.Returning(
            new("João Silva", 100m),
            new("joão silva", 200m),
            new("  JOÃO SILVA  ", 300m));

        var result = await NewService(parser).ImportAsync(content);

        var seller = Assert.Single(_writer.ReceivedSellers);
        Assert.Equal("João Silva", seller.Name);
        Assert.Equal(3, seller.Sales.Count);
        Assert.Equal(new SalesImportSummaryDto(1, 3, 600m), result.Value);
    }

    [Fact]
    public async Task ImportAsync_PassesContentAndTokenThrough()
    {
        using var content = NewContent();
        using var cts = new CancellationTokenSource();
        var parser = FakeSalesParser.Returning(ChallengeSales);

        await NewService(parser).ImportAsync(content, cts.Token);

        Assert.Equal(1, parser.Calls);
        Assert.Same(content, parser.ReceivedContent);
        Assert.Equal(cts.Token, parser.ReceivedToken);
        Assert.Equal(cts.Token, _writer.ReceivedToken);
    }

    [Fact]
    public async Task ImportAsync_DoesNotDisposeContent()
    {
        using var content = NewContent();

        await NewService(FakeSalesParser.Returning(ChallengeSales)).ImportAsync(content);

        Assert.True(content.CanRead);
    }

    [Fact]
    public async Task ImportAsync_WhenParserFails_ReturnsParserErrorAndDoesNotWrite()
    {
        using var content = NewContent();

        var result = await NewService(FakeSalesParser.Failing(SalesImportErrors.InvalidFormat)).ImportAsync(content);

        Assert.True(result.IsFailure);
        Assert.Equal(SalesImportErrors.InvalidFormat, result.Error);
        Assert.Equal(0, _writer.Calls);
    }

    [Fact]
    public async Task ImportAsync_WhenParserReturnsNoSales_ReturnsEmptyAndDoesNotWrite()
    {
        using var content = NewContent();

        var result = await NewService(FakeSalesParser.Returning()).ImportAsync(content);

        Assert.Equal(SalesImportErrors.Empty, result.Error);
        Assert.Equal(0, _writer.Calls);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task ImportAsync_WithEmptySellerName_ReturnsEmptyNameAndDoesNotWrite(string? name)
    {
        using var content = NewContent();
        var parser = FakeSalesParser.Returning(new("João Silva", 100m), new(name!, 200m));

        var result = await NewService(parser).ImportAsync(content);

        Assert.Equal(SellerErrors.EmptyName, result.Error);
        Assert.Equal(0, _writer.Calls);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public async Task ImportAsync_WithInvalidSaleValue_ReturnsInvalidValueAndDoesNotWrite(int value)
    {
        using var content = NewContent();
        var parser = FakeSalesParser.Returning(new("João Silva", 100m), new("João Silva", value));

        var result = await NewService(parser).ImportAsync(content);

        Assert.Equal(SaleErrors.InvalidValue, result.Error);
        Assert.Equal(0, _writer.Calls);
    }
}
