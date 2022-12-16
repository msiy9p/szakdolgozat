namespace Libellus.Domain.Common.Interfaces.Errors;

public interface IError<TId> : IEquatable<IError<TId>>
{
    TId ErrorId { get; }
    string Message { get; }
}

public interface IError : IError<string>
{
}