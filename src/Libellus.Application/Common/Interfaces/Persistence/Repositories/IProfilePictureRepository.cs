using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IProfilePictureReadOnlyRepository
{
    Task<Result<bool>> ExistsAsync(string objectName, CancellationToken cancellationToken = default);

    Task<Result<bool>> ExistsAsync(ProfilePictureId id, CancellationToken cancellationToken = default);

    Task<Result<ProfilePicture>> GetByObjectNameAsync(string objectName, CancellationToken cancellationToken = default);
}

public interface IProfilePictureRepository : IProfilePictureReadOnlyRepository
{
    Task<Result> AddIfNotExistsAsync(ProfilePicture image, CancellationToken cancellationToken = default);

    Task<Result> DeleteByObjectNameAsync(string objectName, CancellationToken cancellationToken = default);

    Task<Result> DeleteByIdAsync(ProfilePictureId id, CancellationToken cancellationToken = default);
}