using Libellus.Domain.Models;

namespace Libellus.Application.Common.Interfaces.Persistence;

public interface IUnitOfWork
{
    Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
}