using Libellus.Application.Enums;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IReadOnlyRepository<TEntity, in TKey> where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>, new()
{
    GroupId CurrentGroupId { get; }

    Task<Result<bool>> ExistsAsync(TKey id, CancellationToken cancellationToken = default);

    Task<Result<TEntity>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    Task<Result<ICollection<TEntity>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default);
}

public interface IRepository<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>, new()
{
    Task<Result<TKey>> AddIfNotExistsAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(TKey id, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}