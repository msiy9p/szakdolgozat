using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Common.Interfaces.Persistence.Repositories;

public interface IUserReadOnlyRepository
{
    Task<Result<bool>> ExistsAsync(UserId id, CancellationToken cancellationToken = default);

    Task<Result<UserVm>> GetVmByIdAsync(UserId id, CancellationToken cancellationToken = default);

    Task<Result<UserPicturedVm>> GetPicturedVmByIdAsync(UserId id, CancellationToken cancellationToken = default);

    Task<Result<ProfilePictureId?>> GetPictureIdAsync(UserId id, CancellationToken cancellationToken = default);

    Task<Result<UserEmailVm>> GetEmailVmByIdAsync(UserId id, CancellationToken cancellationToken = default);

    Task<Result<UserId>> GetIdByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Result<UserId>> GetIdByNameAsync(UserName userName, CancellationToken cancellationToken = default);
}

public interface IUserRepository : IUserReadOnlyRepository
{
    Task<Result> ChangeProfilePictureAsync(UserId id, ProfilePictureId profilePictureId,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveProfilePictureAsync(UserId id, CancellationToken cancellationToken = default);
}