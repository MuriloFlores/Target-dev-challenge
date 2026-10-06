using CommissionManager.Domain.Entities;

namespace CommissionManager.Domain.ValueObjects;

public sealed record Commission
{
    private const decimal MinimumSaleValue = 100m;
    private const decimal HighTierSaleValue = 500m;
    private const decimal LowTierRate = 0.01m;
    private const decimal HighTierRate = 0.05m;
    
    public decimal Rate { get; }
    public decimal Amount { get; }

    private Commission(decimal rate, decimal amount)
    {
        Rate = rate;
        Amount = amount;
    }

    public static Commission For(Sale sale)
    {
        var rate = GetRate(sale.Value);
        var amount = Math.Round(sale.Value * rate, 2, MidpointRounding.AwayFromZero);

        return new Commission(rate, amount);
    }

    private static decimal GetRate(decimal saleValue)
    {
        if (saleValue < MinimumSaleValue)
            return 0m;

        if (saleValue < HighTierSaleValue)
            return LowTierRate;
        
        return HighTierRate;
    }
}