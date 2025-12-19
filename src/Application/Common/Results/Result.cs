public interface IResult
{
    bool Succeeded { get; }
    Error? Error { get; }
}

public class Result : IResult
{
    public bool Succeeded { get; }
    public Error? Error { get; }

    protected Result(bool succeeded, Error? error) { Succeeded = succeeded; Error = error; }
    public static Result Ok() => new(true, null);
    public static Result Fail(Error error) => new(false, error);
}

public class Result<T> : Result, IResult
{
    public T? Value { get; }
    private Result(bool succeeded, T? value, Error? error) : base(succeeded, error) { Value = value; }

    public static Result<T> Ok(T value) => new(true, value, null);
    public static new Result<T> Fail(Error error) => new(false, default, error);
}