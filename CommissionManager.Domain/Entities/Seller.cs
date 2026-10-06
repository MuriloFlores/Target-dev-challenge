using CommissionManager.Domain.Common;
using CommissionManager.Domain.Errors;
using CommissionManager.Domain.ValueObjects;

namespace CommissionManager.Domain.Entities;

public sealed class Seller
{
    private readonly List<Sale> _sales = new();

    public Guid SellerId { get; }
    public string Name { get; }
    public IReadOnlyList<Sale> Sales => _sales.AsReadOnly();
    public decimal TotalSales => _sales.Sum(s => s.Value);
    public decimal TotalCommission => _sales.Sum(s => Commission.For(s).Amount);

    private Seller(string name)
    {
        SellerId = Guid.NewGuid();
        Name = name;
    }

    public static Result<Seller> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return SellerErrors.EmptyName;

        return new Seller(name.Trim());
    }

    public Result AddSale(Sale sale)
    {
        if (!sale.SellerId.Equals(SellerId))
            return Result.Failure(SellerErrors.SaleFromAnotherSeller);

        if (_sales.Any(s => s.SaleId == sale.SaleId))
            return Result.Failure(SellerErrors.DuplicateSale);
        
        _sales.Add(sale);
        return Result.Success();
    }
}