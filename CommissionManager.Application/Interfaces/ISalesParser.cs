using CommissionManager.Application.Dtos;
using CommissionManager.Domain.Common;

namespace CommissionManager.Application.Interfaces;

public interface ISalesParser
{
    Task<Result<IReadOnlyList<SaleInputDto>>> ParserAsync(Stream content, CancellationToken cancellationToken);
}