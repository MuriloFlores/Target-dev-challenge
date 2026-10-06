using CommissionManager.Application.Dtos;
using CommissionManager.Application.Errors;
using CommissionManager.Application.Services;
using CommissionManager.Application.Tests.Fakes;
using CommissionManager.Domain.Errors;

namespace CommissionManager.Application.Tests.Services;

public class CommissionServiceTests
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

    private static CommissionService NewService(FakeSalesReader? reader = null) =>
        new(reader ?? FakeSalesReader.Returning());

    [Fact]
    public void Calculate_WithChallengeData_ReturnsExpectedReport()
    {
        var result = NewService().Calculate(ChallengeSales);

        Assert.True(result.IsSuccess);
        Assert.Equal(
        [
            new SellerCommissionDto("João Silva", 10, 10754.70m, 495.69m),
            new SellerCommissionDto("Maria Souza", 9, 9874.30m, 465.96m),
            new SellerCommissionDto("Ana Lima", 9, 8763.95m, 404.99m),
            new SellerCommissionDto("Carlos Oliveira", 8, 7928.35m, 379.38m),
        ], result.Value.Sellers);
        Assert.Equal(37321.30m, result.Value.TotalSales);
        Assert.Equal(1746.02m, result.Value.TotalCommission);
    }

    [Fact]
    public void Calculate_OrdersSellersByCommissionDescending()
    {
        var result = NewService().Calculate(
        [
            new("Baixa", 200m),
            new("Alta", 2000m),
            new("Media", 600m),
        ]);

        Assert.Equal(["Alta", "Media", "Baixa"], result.Value.Sellers.Select(s => s.SellerName));
    }

    [Fact]
    public void Calculate_GroupsNamesIgnoringCaseAndSurroundingSpaces()
    {
        var result = NewService().Calculate(
        [
            new("João Silva", 100m),
            new("joão silva", 200m),
            new("  JOÃO SILVA  ", 300m),
        ]);

        var seller = Assert.Single(result.Value.Sellers);
        Assert.Equal("João Silva", seller.SellerName);
        Assert.Equal(3, seller.SalesCount);
        Assert.Equal(600m, seller.TotalSales);
    }

    [Fact]
    public void Calculate_SellerWithOnlySalesBelowThreshold_HasZeroCommission()
    {
        var result = NewService().Calculate([new("Ana Lima", 75.30m), new("Ana Lima", 99.99m)]);

        var seller = Assert.Single(result.Value.Sellers);
        Assert.Equal(0m, seller.TotalCommission);
    }

    [Fact]
    public void Calculate_WithNoSales_ReturnsEmptyError()
    {
        var result = NewService().Calculate([]);

        Assert.True(result.IsFailure);
        Assert.Equal(SalesImportErrors.Empty, result.Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Calculate_WithEmptySellerName_ReturnsEmptyNameError(string? name)
    {
        var result = NewService().Calculate([new(name!, 100m)]);

        Assert.True(result.IsFailure);
        Assert.Equal(SellerErrors.EmptyName, result.Error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void Calculate_WithInvalidSaleValue_ReturnsInvalidValueError(int value)
    {
        var result = NewService().Calculate([new("João Silva", 100m), new("João Silva", value)]);

        Assert.True(result.IsFailure);
        Assert.Equal(SaleErrors.InvalidValue, result.Error);
    }

    [Fact]
    public async Task CalculateAsync_ReadsSalesFromReader()
    {
        var reader = FakeSalesReader.Returning(ChallengeSales);

        var result = await NewService(reader).CalculateAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(1, reader.Calls);
        Assert.Equal(4, result.Value.Sellers.Count);
        Assert.Equal(1746.02m, result.Value.TotalCommission);
    }

    [Fact]
    public async Task CalculateAsync_WhenReaderFails_ReturnsReaderError()
    {
        var reader = FakeSalesReader.Failing(SalesImportErrors.SourceNotFound);

        var result = await NewService(reader).CalculateAsync();

        Assert.True(result.IsFailure);
        Assert.Equal(SalesImportErrors.SourceNotFound, result.Error);
    }

    [Fact]
    public async Task CalculateAsync_WhenReaderReturnsNoSales_ReturnsEmptyError()
    {
        var result = await NewService(FakeSalesReader.Returning()).CalculateAsync();

        Assert.Equal(SalesImportErrors.Empty, result.Error);
    }

    [Fact]
    public async Task CalculateAsync_PassesCancellationTokenToReader()
    {
        var reader = FakeSalesReader.Returning(ChallengeSales);
        using var cts = new CancellationTokenSource();

        await NewService(reader).CalculateAsync(cts.Token);

        Assert.Equal(cts.Token, reader.ReceivedToken);
    }
}
