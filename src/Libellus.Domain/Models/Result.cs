using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Errors;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Enums;

namespace Libellus.Domain.Models;

public class Result : IResult
{
    public static readonly Result Succeeded = new();

    private readonly List<IError> _errors = new();

    public ResultStatus Status { get; init; }
    public bool IsSuccess => Status == ResultStatus.Ok;
    public bool IsError => !IsSuccess;
    public IReadOnlyCollection<IError> Errors => _errors.AsReadOnly();

    protected Result()
    {
        Status = ResultStatus.Ok;
    }

    protected Result(ResultStatus status, IError error)
    {
        Guard.Against.Null(error);

        Status = status;
        _errors.Add(error);
    }

    protected Result(ResultStatus status, params IError[] errors)
    {
        Guard.Against.Null(errors);
        Guard.Against.Zero(errors.Length);

        _errors.Capacity = errors.Length;
        foreach (IError error in errors)
        {
            Guard.Against.Null(error);
            _errors.Add(error);
        }

        Status = status;
    }

    protected Result(ResultStatus status, IEnumerable<IError> errors)
    {
        Guard.Against.Null(errors);

        foreach (IError error in errors)
        {
            Guard.Against.Null(error);
            _errors.Add(error);
        }

        Status = status;

        Guard.Against.Zero(_errors.Count);
    }

    public static Result Success() => Succeeded;

    public static Result Error(IError error)
    {
        return new Result(ResultStatus.Error, error);
    }

    public static Result Error(params IError[] errors)
    {
        return new Result(ResultStatus.Error, errors);
    }

    public static Result Error(IEnumerable<IError> errors)
    {
        return new Result(ResultStatus.Error, errors);
    }

    public static Result Invalid(IError error)
    {
        return new Result(ResultStatus.Invalid, error);
    }

    public static Result Invalid(params IError[] errors)
    {
        return new Result(ResultStatus.Invalid, errors);
    }

    public static Result Invalid(IEnumerable<IError> errors)
    {
        return new Result(ResultStatus.Invalid, errors);
    }
}

public class Result<TValue> : Result, IResult<TValue>
{
    private readonly TValue _value = default!;

    public TValue Value
    {
        get
        {
            if (IsError)
            {
                throw new InvalidOperationException("Result is not successful.");
            }

            return _value;
        }
    }

    protected Result(TValue value) : base()
    {
        _value = value;
    }

    protected Result(ResultStatus status, IError error) : base(status, error)
    {
    }

    protected Result(ResultStatus status, params IError[] errors) : base(status, errors)
    {
    }

    protected Result(ResultStatus status, IEnumerable<IError> errors) : base(status, errors)
    {
    }

    public static implicit operator TValue(Result<TValue> result)
    {
        if (result.IsError)
        {
            throw new InvalidOperationException("Result is not successful.");
        }

        return result._value;
    }

    public static Result<TValue> Success(TValue value)
    {
        return new Result<TValue>(value);
    }

    public new static Result<TValue> Error(IError error)
    {
        return new Result<TValue>(ResultStatus.Error, error);
    }

    public new static Result<TValue> Error(params IError[] errors)
    {
        return new Result<TValue>(ResultStatus.Error, errors);
    }

    public new static Result<TValue> Error(IEnumerable<IError> errors)
    {
        return new Result<TValue>(ResultStatus.Error, errors);
    }

    public new static Result<TValue> Invalid(IError error)
    {
        return new Result<TValue>(ResultStatus.Invalid, error);
    }

    public new static Result<TValue> Invalid(params IError[] errors)
    {
        return new Result<TValue>(ResultStatus.Invalid, errors);
    }

    public new static Result<TValue> Invalid(IEnumerable<IError> errors)
    {
        return new Result<TValue>(ResultStatus.Invalid, errors);
    }
}