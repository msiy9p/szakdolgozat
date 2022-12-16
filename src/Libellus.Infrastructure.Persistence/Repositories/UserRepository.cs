using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Libellus.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Libellus.Domain.Errors.DomainErrors;
using DomainProfilePictureMetaDataContainer = Libellus.Domain.Entities.ProfilePictureMetaDataContainer;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository : BaseRepository<UserRepository>, IUserRepository
{
    public UserRepository(ApplicationContext context, ILogger<UserRepository> logger) : base(context, logger)
    {
    }

    internal UserRepository(ApplicationContext context, ILogger logger) : base(context, logger)
    {
    }

    private async Task<DomainProfilePictureMetaDataContainer?> GetProfilePicturesAsync(
        ProfilePictureId profilePictureId, CancellationToken cancellationToken = default)
    {
        IProfilePictureMetaDataReadOnlyRepository profilePictureRepo =
            new ProfilePictureMetadataRepository(Context, Logger);
        var results = await profilePictureRepo.GetByIdAsync(profilePictureId, cancellationToken);

        if (results.IsError)
        {
            return null;
        }

        return new DomainProfilePictureMetaDataContainer(profilePictureId, results.Value!);
    }

    public async Task<Result<bool>> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.Users
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<UserVm>> GetVmByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.Users
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<UserVm>();
        }

        return UserVm.Create(found.Id, found.UserName!);
    }

    public async Task<Result<UserPicturedVm>> GetPicturedVmByIdAsync(UserId id,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Users
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<UserPicturedVm>();
        }

        if (found.ProfilePictureId.HasValue)
        {
            var profilePicture = await GetProfilePicturesAsync(found.ProfilePictureId.Value, cancellationToken);
            return UserPicturedVm.Create(found.Id, found.UserName!, profilePicture);
        }

        return UserPicturedVm.Create(found.Id, found.UserName!, null);
    }

    public async Task<Result<ProfilePictureId?>> GetPictureIdAsync(UserId id,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Users
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<ProfilePictureId?>();
        }

        return found.ProfilePictureId.ToResult();
    }

    public async Task<Result<UserEmailVm>> GetEmailVmByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.Users
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<UserEmailVm>();
        }

        return UserEmailVm.Create(found.UserName!, found.Email!);
    }

    public async Task<Result<UserId>> GetIdByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var found = await Context.Users
            .Where(x => x.Email == email)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (found == default)
        {
            return UserErrors.UserNotFound.ToErrorResult<UserId>();
        }

        return found.ToResult();
    }

    public async Task<Result<UserId>> GetIdByNameAsync(UserName userName, CancellationToken cancellationToken = default)
    {
        var found = await Context.Users
            .Where(x => x.NormalizedUserName == userName.ValueNormalized)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (found == default)
        {
            return UserErrors.UserNotFound.ToErrorResult<UserId>();
        }

        return found.ToResult();
    }

    public async Task<Result> ChangeProfilePictureAsync(UserId id, ProfilePictureId profilePictureId,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Users
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<UserEmailVm>();
        }

        IProfilePictureMetaDataRepository profilePictureRepository =
            new ProfilePictureMetadataRepository(Context, Logger);

        var exists = await profilePictureRepository.GetByIdAsContainerAsync(profilePictureId, cancellationToken);
        if (exists.IsError)
        {
            return exists;
        }

        if (found.ProfilePictureId.HasValue && found.ProfilePictureId.Value != profilePictureId)
        {
            var deleteResult =
                await profilePictureRepository.DeleteAsync(found.ProfilePictureId.Value, cancellationToken);
            if (deleteResult.IsError)
            {
                return deleteResult;
            }

            found.ProfilePictureId = exists.Value.Id;
        }
        else
        {
            found.ProfilePictureId = exists.Value.Id;
        }

        Context.Users.Update(found);

        return Result.Success();
    }

    public async Task<Result> RemoveProfilePictureAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.Users
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return UserErrors.UserNotFound.ToErrorResult<UserEmailVm>();
        }

        found.ProfilePictureId = null;

        Context.Users.Update(found);

        return Result.Success();
    }
}