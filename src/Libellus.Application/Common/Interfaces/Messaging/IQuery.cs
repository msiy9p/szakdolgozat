using Libellus.Domain.Models;
using MediatR;

namespace Libellus.Application.Common.Interfaces.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}