using CommissionManager.Domain.Common;
using CommissionManager.Domain.Errors;

namespace CommissionManager.Domain.Entities;

public sealed class Sale
{
    public Guid SaleId { get; }
    public Guid SellerId { get; }
    public decimal Value  { get; }
    
    private Sale(Guid sellerId, decimal value)
    {
        SaleId = Guid.NewGuid();
        SellerId = sellerId;
        Value = value;
    }

    public static Result<Sale> Create(Guid sellerId, decimal value)
    {
        if (sellerId == Guid.Empty)
            return SaleErrors.EmptySellerId;

        if (value <= 0)
            return SaleErrors.InvalidValue;

        return new Sale(sellerId, value);
    }
}