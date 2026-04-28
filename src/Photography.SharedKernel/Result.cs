namespace Photography.SharedKernel;

/// <summary>
/// Result type for operations that can succeed or fail with a known domain reason.
/// Application services and MediatR handlers should return Result/Result&lt;T&gt; rather than
/// throwing exceptions for expected failure cases.
/// </summary>
public class Result
{
    public bool Success { get; }
    public bool IsSuccess => Success;
    public bool IsFailed => !Success;
    public string? Error { get; }
    public ResultErrorKind ErrorKind { get; }

    protected Result(bool success, string? error, ResultErrorKind errorKind)
    {
        Success = success;
        Error = error;
        ErrorKind = errorKind;
    }

    public static Result Ok() => new(true, null, ResultErrorKind.None);
    public static Result Fail(string error, ResultErrorKind kind = ResultErrorKind.Validation) => new(false, error, kind);
    public static Result NotFound(string error) => new(false, error, ResultErrorKind.NotFound);
    public static Result Conflict(string error) => new(false, error, ResultErrorKind.Conflict);
    public static Result Unauthorized(string error) => new(false, error, ResultErrorKind.Unauthorized);
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool success, T? value, string? error, ResultErrorKind kind)
        : base(success, error, kind)
    {
        Value = value;
    }

    public static Result<T> Ok(T value) => new(true, value, null, ResultErrorKind.None);
    public static new Result<T> Fail(string error, ResultErrorKind kind = ResultErrorKind.Validation)
        => new(false, default, error, kind);
    public static new Result<T> NotFound(string error) => new(false, default, error, ResultErrorKind.NotFound);
    public static new Result<T> Conflict(string error) => new(false, default, error, ResultErrorKind.Conflict);
    public static new Result<T> Unauthorized(string error) => new(false, default, error, ResultErrorKind.Unauthorized);
}

public enum ResultErrorKind
{
    None = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4,
    Forbidden = 5,
    Unexpected = 99
}
