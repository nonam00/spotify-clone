namespace Application.Shared.Data;

public class Result
{
    public bool IsSuccess { get; }
    public Error Error { get; }

    public bool IsFailure => !IsSuccess;


    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }
        
        IsSuccess = isSuccess;
        Error = error;
    }
    
    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);
}

public class Result<T> : Result
{
    private readonly T _value;
    
    public T Value => IsSuccess ? _value : throw new InvalidOperationException("Cannot access Value of failed result");

    private Result(T value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        _value = value;
    }
    
    public static Result<T> Success(T value) => new(value, true, Error.None);

    public new static Result<T> Failure(Error error) => new(default!, false, error);
}