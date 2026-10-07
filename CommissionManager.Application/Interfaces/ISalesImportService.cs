using CommissionManager.Application.Dtos;
using CommissionManager.Domain.Common;

namespace CommissionManager.Application.Interfaces;

public interface ISalesImportService
{
    Task<Result<SalesImportSummaryDto>> ImportAsync(Stream content, CancellationToken cancellationToken = default);
}