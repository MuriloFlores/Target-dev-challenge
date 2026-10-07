using CommissionManager.Application.Dtos;
using CommissionManager.Application.Interfaces;
using CommissionManager.Domain.Common;

namespace CommissionManager.Application.Tests.Fakes;

public sealed class FakeSalesParser(Result<IReadOnlyList<SaleInputDto>> result) : ISalesParser
{
    public int Calls { get; private set; }
    public Stream? ReceivedContent { get; private set; }
    public CancellationToken ReceivedToken { get; private set; }

    public static FakeSalesParser Returning(params SaleInputDto[] sales) =>
        new(Result<IReadOnlyList<SaleInputDto>>.Success(sales));

    public static FakeSalesParser Failing(Error error) => new(error);

    public Task<Result<IReadOnlyList<SaleInputDto>>> ParserAsync(Stream content, CancellationToken cancellationToken)
    {
        Calls++;
        ReceivedContent = content;
        ReceivedToken = cancellationToken;
        return Task.FromResult(result);
    }
}
