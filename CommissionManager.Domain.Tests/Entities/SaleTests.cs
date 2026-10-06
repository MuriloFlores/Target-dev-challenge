using CommissionManager.Domain.Entities;
using CommissionManager.Domain.Errors;

namespace CommissionManager.Domain.Tests.Entities;

public class SaleTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSale()
    {
        var sellerId = Guid.NewGuid();

        var result = Sale.Create(sellerId, 1200.50m);

        Assert.True(result.IsSuccess);
        Assert.Equal(sellerId, result.Value.SellerId);
        Assert.Equal(1200.50m, result.Value.Value);
        Assert.NotEqual(Guid.Empty, result.Value.SaleId);
    }

    [Fact]
    public void Create_GeneratesDifferentIdsForEachSale()
    {
        var sellerId = Guid.NewGuid();

        var first = Sale.Create(sellerId, 100m).Value;
        var second = Sale.Create(sellerId, 100m).Value;

        Assert.NotEqual(first.SaleId, second.SaleId);
    }

    [Fact]
    public void Create_WithEmptySellerId_ReturnsEmptySellerIdError()
    {
        var result = Sale.Create(Guid.Empty, 100m);

        Assert.True(result.IsFailure);
        Assert.Equal(SaleErrors.EmptySellerId, result.Error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-1200.50)]
    public void Create_WithValueZeroOrNegative_ReturnsInvalidValueError(double value)
    {
        var result = Sale.Create(Guid.NewGuid(), (decimal)value);

        Assert.True(result.IsFailure);
        Assert.Equal(SaleErrors.InvalidValue, result.Error);
    }

    [Fact]
    public void Create_WithValueBelowCommissionThreshold_IsStillValid()
    {
        var result = Sale.Create(Guid.NewGuid(), 0.01m);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_WithEmptySellerIdAndInvalidValue_ReturnsSellerIdErrorFirst()
    {
        var result = Sale.Create(Guid.Empty, -1m);

        Assert.Equal(SaleErrors.EmptySellerId, result.Error);
    }
}
