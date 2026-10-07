using CommissionManager.Application.Dtos;
using CommissionManager.Application.Errors;
using CommissionManager.Domain.Common;
using CommissionManager.Domain.Entities;

namespace CommissionManager.Application.Services;

internal static class SellerBuilder
{
    public static Result<IReadOnlyList<Seller>> FromSales(IReadOnlyList<SaleInputDto> sales)
    {
        if (sales.Count == 0)
            return SalesImportErrors.Empty;

        var sellers = new List<Seller>();

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

            sellers.Add(seller);
        }

        return Result<IReadOnlyList<Seller>>.Success(sellers);
    }
}