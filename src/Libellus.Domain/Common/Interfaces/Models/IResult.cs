using Libellus.Domain.Common.Interfaces.Errors;
using Libellus.Domain.Enums;

namespace Libellus.Domain.Common.Interfaces.Models;

public interface IResult
{
    bool IsError { get; }
    ResultStatus Status { get; }
    IReadOnlyCollection<IError> Errors { get; }
}

public interface IResult<out TValue> : IResult
{
    TValue Value { get; }
}