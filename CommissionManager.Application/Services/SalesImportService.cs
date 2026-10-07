using CommissionManager.Application.Dtos;
using CommissionManager.Application.Interfaces;
using CommissionManager.Domain.Common;

namespace CommissionManager.Application.Services;

public sealed class SalesImportService(ISalesParser parser, ISalesWriter writer) : ISalesImportService
{
    public async Task<Result<SalesImportSummaryDto>> ImportAsync(Stream content,
        CancellationToken cancellationToken = default)
    {
        var parseResult = await parser.ParserAsync(content, cancellationToken);
        if (parseResult.IsFailure)
            return parseResult.Error;
        
        var sellerResult = SellerBuilder.FromSales(parseResult.Value);
        if (sellerResult.IsFailure)
            return sellerResult.Error;
        
        var sellers = sellerResult.Value;
        await writer.AddAsync(sellers, cancellationToken);
        
        return SalesImportSummaryDto.From(sellers);
    }
}