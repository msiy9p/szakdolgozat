using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface ICoverImageReadOnlyRepository
{
    Task<Result<bool>> ExistsAsync(string objectName, CancellationToken cancellationToken = default);

    Task<Result<bool>> ExistsAsync(CoverImageId id, CancellationToken cancellationToken = default);

    Task<Result<CoverImage>> GetByObjectNameAsync(string objectName, CancellationToken cancellationToken = default);
}

public interface ICoverImageRepository : ICoverImageReadOnlyRepository
{
    Task<Result> AddIfNotExistsAsync(CoverImage image, CancellationToken cancellationToken = default);

    Task<Result> DeleteByObjectNameAsync(string objectName, CancellationToken cancellationToken = default);

    Task<Result> DeleteByIdAsync(CoverImageId id, CancellationToken cancellationToken = default);
}