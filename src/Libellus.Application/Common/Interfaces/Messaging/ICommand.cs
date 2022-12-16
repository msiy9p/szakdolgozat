using Libellus.Domain.Models;
using MediatR;

namespace Libellus.Application.Common.Interfaces.Messaging;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

public interface ICommand : IRequest<Result>
{
}