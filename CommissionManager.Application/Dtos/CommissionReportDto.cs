namespace CommissionManager.Application.Dtos;

public record CommissionReportDto(IReadOnlyList<SellerCommissionDto> Sellers)
{
    public decimal TotalSales => Sellers.Sum(s => s.TotalSales);
    public decimal TotalCommission => Sellers.Sum(s => s.TotalCommission);
}