using CommissionManager.Domain.Entities;
using CommissionManager.Domain.ValueObjects;

namespace CommissionManager.Domain.Tests.ValueObjects;

public class CommissionTests
{
    private static Sale NewSale(decimal value) => Sale.Create(Guid.NewGuid(), value).Value;

    public static TheoryData<decimal, decimal, decimal> Tiers => new()
    {
        { 0.01m, 0m, 0m },
        { 99.99m, 0m, 0m },
        { 100.00m, 0.01m, 1.00m },
        { 250.30m, 0.01m, 2.50m },
        { 499.99m, 0.01m, 5.00m },
        { 500.00m, 0.05m, 25.00m },
        { 1800.00m, 0.05m, 90.00m },
    };

    [Theory]
    [MemberData(nameof(Tiers))]
    public void For_AppliesRateOfTier(decimal saleValue, decimal expectedRate, decimal expectedAmount)
    {
        var commission = Commission.For(NewSale(saleValue));

        Assert.Equal(expectedRate, commission.Rate);
        Assert.Equal(expectedAmount, commission.Amount);
    }

    public static TheoryData<decimal, decimal> MidpointCases => new()
    {
        { 400.50m, 4.01m },
        { 1200.50m, 60.03m },
        { 1550.90m, 77.55m },
    };

    [Theory]
    [MemberData(nameof(MidpointCases))]
    public void For_RoundsMidpointAwayFromZero(decimal saleValue, decimal expectedAmount)
    {
        Assert.Equal(expectedAmount, Commission.For(NewSale(saleValue)).Amount);
    }

    [Fact]
    public void For_AlwaysRoundsToTwoDecimalPlaces()
    {
        var commission = Commission.For(NewSale(950.75m));

        Assert.Equal(47.54m, commission.Amount);
        Assert.Equal(2, commission.Amount.Scale);
    }

    [Fact]
    public void Commissions_WithSameRateAndAmount_AreEqual()
    {
        Assert.Equal(Commission.For(NewSale(1000m)), Commission.For(NewSale(1000m)));
    }
}
