using CommissionManager.Domain.Common;

namespace CommissionManager.Domain.Tests.Common;

public class ErrorTests
{
    [Fact]
    public void None_HasEmptyCodeAndMessageAndTypeNone()
    {
        Assert.Equal(string.Empty, Error.None.Code);
        Assert.Equal(string.Empty, Error.None.Message);
        Assert.Equal(ErrorType.None, Error.None.Type);
    }

    [Fact]
    public void Validation_SetsTypeValidation()
    {
        var error = Error.Validation("Code", "Message");

        Assert.Equal("Code", error.Code);
        Assert.Equal("Message", error.Message);
        Assert.Equal(ErrorType.Validation, error.Type);
    }

    [Fact]
    public void NotFound_SetsTypeNotFound()
    {
        Assert.Equal(ErrorType.NotFound, Error.NotFound("Code", "Message").Type);
    }

    [Fact]
    public void Conflict_SetsTypeConflict()
    {
        Assert.Equal(ErrorType.Conflict, Error.Conflict("Code", "Message").Type);
    }

    [Fact]
    public void Failure_SetsTypeFailure()
    {
        Assert.Equal(ErrorType.Failure, Error.Failure("Code", "Message").Type);
    }

    [Fact]
    public void Errors_WithSameData_AreEqual()
    {
        Assert.Equal(Error.Validation("Code", "Message"), Error.Validation("Code", "Message"));
    }

    [Fact]
    public void Errors_WithSameCodeButDifferentType_AreNotEqual()
    {
        Assert.NotEqual(Error.Validation("Code", "Message"), Error.Conflict("Code", "Message"));
    }
}
