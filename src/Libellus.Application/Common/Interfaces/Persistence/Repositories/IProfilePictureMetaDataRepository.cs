using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IProfilePictureMetaDataReadOnlyRepository
{
    Task<Result<bool>> ExistsAsync(ProfilePictureId id, CancellationToken cancellationToken = default);

    Task<Result<ICollection<ProfilePictureMetaData>>> GetByIdAsync(ProfilePictureId id,
        CancellationToken cancellationToken = default);

    Task<Result<ProfilePictureMetaDataContainer>> GetByIdAsContainerAsync(ProfilePictureId id,
        CancellationToken cancellationToken = default);
}

public interface IProfilePictureMetaDataRepository : IProfilePictureMetaDataReadOnlyRepository
{
    Task<Result> AddAsync(ProfilePictureMetaDataContainer coverImageMetaDataContainer,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(ProfilePictureId coverImageId, CancellationToken cancellationToken = default);
}