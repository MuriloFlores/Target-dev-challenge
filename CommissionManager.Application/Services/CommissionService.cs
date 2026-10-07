using CommissionManager.Application.Dtos;
using CommissionManager.Application.Interfaces;
using CommissionManager.Domain.Common;

namespace CommissionManager.Application.Services;

public sealed class CommissionService(ISalesReader salesReader) : ICommissionService
{
    public async Task<Result<CommissionReportDto>> CalculateAsync(CancellationToken cancellationToken = default)
    {
        var salesResult = await salesReader.ReadAllAsync(cancellationToken);
        if (salesResult.IsFailure)
            return salesResult.Error;

        return Calculate(salesResult.Value);
    }

    public Result<CommissionReportDto> Calculate(IReadOnlyList<SaleInputDto> sales)
    {
        var sellersResult = SellerBuilder.FromSales(sales);
        if (sellersResult.IsFailure)
            return sellersResult.Error;

        return new CommissionReportDto(
            sellersResult.Value
                .Select(SellerCommissionDto.From)
                .OrderByDescending(s => s.TotalCommission)
                .ToList());
    }
}