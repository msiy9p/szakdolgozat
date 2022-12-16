using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface ICoverImageMetaDataReadOnlyRepository
{
    Task<Result<bool>> ExistsAsync(CoverImageId id, CancellationToken cancellationToken = default);

    Task<Result<ICollection<CoverImageMetaData>>> GetByIdAsync(CoverImageId id,
        CancellationToken cancellationToken = default);

    Task<Result<CoverImageMetaDataContainer>> GetByIdAsContainerAsync(CoverImageId id,
        CancellationToken cancellationToken = default);
}

public interface ICoverImageMetaDataRepository : ICoverImageMetaDataReadOnlyRepository
{
    Task<Result> AddAsync(CoverImageMetaDataContainer coverImageMetaDataContainer,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(CoverImageId coverImageId, CancellationToken cancellationToken = default);
}