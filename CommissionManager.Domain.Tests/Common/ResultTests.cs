using CommissionManager.Domain.Common;

namespace CommissionManager.Domain.Tests.Common;

public class ResultTests
{
    private static readonly Error SomeError = Error.Validation("Test.Error", "Erro de teste");

    private sealed class ExposedResult(bool isSuccess, Error error) : Result(isSuccess, error);

    [Fact]
    public void Success_IsSuccessWithNoError()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Failure_IsFailureWithGivenError()
    {
        var result = Result.Failure(SomeError);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(SomeError, result.Error);
    }

    [Fact]
    public void Constructor_SuccessWithError_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => new ExposedResult(true, SomeError));
    }

    [Fact]
    public void Constructor_FailureWithoutError_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => new ExposedResult(false, Error.None));
    }
}
