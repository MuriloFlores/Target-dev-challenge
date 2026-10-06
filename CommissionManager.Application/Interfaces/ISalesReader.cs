using CommissionManager.Application.Dtos;
using CommissionManager.Domain.Common;

namespace CommissionManager.Application.Interfaces;

public interface ISalesReader
{
    Task<Result<IReadOnlyList<SaleInputDto>>> ReadAllAsync(CancellationToken cancellationToken = default);
}