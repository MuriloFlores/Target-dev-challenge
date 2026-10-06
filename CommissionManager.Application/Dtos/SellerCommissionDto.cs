using CommissionManager.Domain.Entities;

namespace CommissionManager.Application.Dtos;

public sealed record SellerCommissionDto(
    string SellerName,
    int SalesCount,
    decimal TotalSales,
    decimal TotalCommission)
{
    public static SellerCommissionDto From(Seller seller) =>
        new(seller.Name, seller.Sales.Count, seller.TotalSales, seller.TotalCommission);
}