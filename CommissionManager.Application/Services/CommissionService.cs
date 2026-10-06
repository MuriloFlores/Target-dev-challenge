using CommissionManager.Application.Dtos;
using CommissionManager.Application.Errors;
using CommissionManager.Application.Interfaces;
using CommissionManager.Domain.Common;
using CommissionManager.Domain.Entities;

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
        if (sales.Count == 0)
            return SalesImportErrors.Empty;

        var sellers = new List<SellerCommissionDto>();

        var salesBySeller = sales.GroupBy(
            s => s.SellerName?.Trim() ?? string.Empty,
            StringComparer.OrdinalIgnoreCase);

        foreach (var group in salesBySeller)
        {
            var sellerResult = Seller.Create(group.Key);
            if (sellerResult.IsFailure)
                return sellerResult.Error;

            var seller = sellerResult.Value;

            foreach (var input in group)
            {
                var saleResult = Sale.Create(seller.SellerId, input.Value);
                if (saleResult.IsFailure)
                    return saleResult.Error;

                var addResult = seller.AddSale(saleResult.Value);
                if (addResult.IsFailure)
                    return addResult.Error;
            }

            sellers.Add(SellerCommissionDto.From(seller));
        }

        return new CommissionReportDto(
            sellers.OrderByDescending(s => s.TotalCommission).ToList());
    }
}