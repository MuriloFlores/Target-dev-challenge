using CommissionManager.Domain.Common;

namespace CommissionManager.Domain.Tests.Common;

public class ResultOfTTests
{
    private static readonly Error SomeError = Error.Validation("Test.Error", "Erro de teste");

    [Fact]
    public void Success_ExposesValue()
    {
        var result = Result<int>.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Failure_ExposesError()
    {
        var result = Result<int>.Failure(SomeError);

        Assert.True(result.IsFailure);
        Assert.Equal(SomeError, result.Error);
    }

    [Fact]
    public void Value_OnFailure_Throws()
    {
        var result = Result<int>.Failure(SomeError);

        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void ImplicitConversion_FromValue_CreatesSuccess()
    {
        Result<string> result = "ok";

        Assert.True(result.IsSuccess);
        Assert.Equal("ok", result.Value);
    }

    [Fact]
    public void ImplicitConversion_FromError_CreatesFailure()
    {
        Result<string> result = SomeError;

        Assert.True(result.IsFailure);
        Assert.Equal(SomeError, result.Error);
    }
}
