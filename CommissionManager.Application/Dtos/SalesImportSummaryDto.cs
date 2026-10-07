using CommissionManager.Domain.Entities;

namespace CommissionManager.Application.Dtos;

public sealed record SalesImportSummaryDto(int SellersCount, int SalesCount, decimal TotalValue)
{
    public static SalesImportSummaryDto From(IReadOnlyCollection<Seller> sellers) =>
        new(sellers.Count, sellers.Sum(s => s.Sales.Count), sellers.Sum(s => s.TotalSales));
}