using CommissionManager.Application.Dtos;
using CommissionManager.Domain.Common;

namespace CommissionManager.Application.Interfaces;

public interface ICommissionService
{
    Task<Result<CommissionReportDto>> CalculateAsync(CancellationToken cancellationToken = default);
    Result<CommissionReportDto> Calculate(IReadOnlyList<SaleInputDto> sales);
}