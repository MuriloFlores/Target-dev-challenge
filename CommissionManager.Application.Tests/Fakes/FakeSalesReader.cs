using CommissionManager.Application.Dtos;
using CommissionManager.Application.Interfaces;
using CommissionManager.Domain.Common;

namespace CommissionManager.Application.Tests.Fakes;

public sealed class FakeSalesReader(Result<IReadOnlyList<SaleInputDto>> result) : ISalesReader
{
    public int Calls { get; private set; }
    public CancellationToken ReceivedToken { get; private set; }

    public static FakeSalesReader Returning(params SaleInputDto[] sales) =>
        new(Result<IReadOnlyList<SaleInputDto>>.Success(sales));

    public static FakeSalesReader Failing(Error error) => new(error);

    public Task<Result<IReadOnlyList<SaleInputDto>>> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        Calls++;
        ReceivedToken = cancellationToken;
        return Task.FromResult(result);
    }
}
