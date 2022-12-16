using Ardalis.GuardClauses;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ViewModels;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Microsoft.Extensions.Logging;
using DomainCoverImageMetaData = Libellus.Domain.Entities.CoverImageMetaData;

namespace Libellus.Infrastructure.Persistence.Repositories.Common;

internal abstract class BaseGroupedRepository<TRepository, TModel> : BaseRepository<TRepository>
    where TRepository : BaseRepository where TModel : class
{
    public GroupId CurrentGroupId { get; init; }

    protected BaseGroupedRepository(ApplicationContext context, ICurrentGroupService currentGroupService,
        ILogger<TRepository> logger) : base(context, logger)
    {
        CurrentGroupId = Guard.Against.Null(currentGroupService.CurrentGroupId)!.Value;
    }

    protected BaseGroupedRepository(ApplicationContext context, GroupId currentGroupId, ILogger logger) : base(context,
        logger)
    {
        CurrentGroupId = currentGroupId;
    }

    protected abstract IQueryable<TModel> GetFiltered();

    protected async Task<ICollection<DomainCoverImageMetaData>> GetCoversAsync(CoverImageId coverImageId,
        CancellationToken cancellationToken = default)
    {
        ICoverImageMetaDataReadOnlyRepository coverImageRepo = new CoverImageMetadataRepository(Context, Logger);
        var results = await coverImageRepo.GetByIdAsync(coverImageId, cancellationToken);

        if (results.IsError)
        {
            return Array.Empty<DomainCoverImageMetaData>();
        }

        return results.Value!;
    }

    protected async Task<Result> AddCoversAsync(CoverImageMetaDataContainer coverImageMetaDataContainer,
        CancellationToken cancellationToken = default)
    {
        ICoverImageMetaDataRepository coverImageRepo = new CoverImageMetadataRepository(Context, Logger);
        return await coverImageRepo.AddAsync(coverImageMetaDataContainer, cancellationToken);
    }

    protected async Task<Result> DeleteCoversAsync(CoverImageId coverImageId,
        CancellationToken cancellationToken = default)
    {
        ICoverImageMetaDataRepository coverImageRepo = new CoverImageMetadataRepository(Context, Logger);
        return await coverImageRepo.DeleteAsync(coverImageId, cancellationToken);
    }

    protected async Task<UserVm?> GetUserVmAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        IUserReadOnlyRepository userRepository = new UserRepository(Context, Logger);
        var result = await userRepository.GetVmByIdAsync(userId, cancellationToken);
        if (result.IsError)
        {
            return null;
        }

        return result.Value;
    }

    protected async Task<UserPicturedVm?> GetUserPicturedVmAsync(UserId userId,
        CancellationToken cancellationToken = default)
    {
        IUserReadOnlyRepository userRepository = new UserRepository(Context, Logger);
        var result = await userRepository.GetPicturedVmByIdAsync(userId, cancellationToken);
        if (result.IsError)
        {
            return null;
        }

        return result.Value;
    }
}