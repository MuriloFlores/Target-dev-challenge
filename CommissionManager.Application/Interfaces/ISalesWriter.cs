using CommissionManager.Domain.Entities;

namespace CommissionManager.Application.Interfaces;

public interface ISalesWriter
{
    Task AddAsync(IReadOnlyCollection<Seller> sellers, CancellationToken cancellationToken);
}