using Libellus.Domain.Models;
using MediatR;

namespace Libellus.Application.Common.Interfaces.Messaging;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}