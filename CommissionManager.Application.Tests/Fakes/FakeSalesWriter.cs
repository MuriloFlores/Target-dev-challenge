using CommissionManager.Application.Interfaces;
using CommissionManager.Domain.Entities;

namespace CommissionManager.Application.Tests.Fakes;

public sealed class FakeSalesWriter : ISalesWriter
{
    public int Calls { get; private set; }
    public IReadOnlyCollection<Seller> ReceivedSellers { get; private set; } = [];
    public CancellationToken ReceivedToken { get; private set; }

    public Task AddAsync(IReadOnlyCollection<Seller> sellers, CancellationToken cancellationToken)
    {
        Calls++;
        ReceivedSellers = sellers;
        ReceivedToken = cancellationToken;
        return Task.CompletedTask;
    }
}
