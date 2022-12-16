using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Libellus.Infrastructure.Persistence.Mapping;
using Libellus.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class ProfilePictureMetadataRepository : BaseRepository<ProfilePictureMetadataRepository>,
    IProfilePictureMetaDataRepository
{
    public ProfilePictureMetadataRepository(ApplicationContext context,
        ILogger<ProfilePictureMetadataRepository> logger) : base(context, logger)
    {
    }

    internal ProfilePictureMetadataRepository(ApplicationContext context, ILogger logger) : base(context, logger)
    {
    }

    public async Task<Result<bool>> ExistsAsync(ProfilePictureId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.ProfilePictures
            .AnyAsync(x => x.PublicId == id.Value, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<ICollection<ProfilePictureMetaData>>> GetByIdAsync(ProfilePictureId id,
        CancellationToken cancellationToken = default)
    {
        var temp = await Context.ProfilePictures
            .Where(x => x.PublicId == id.Value)
            .OrderBy(x => x.Width)
            .ThenBy(x => x.MimeType)
            .ToListAsync(cancellationToken);

        var output = new List<ProfilePictureMetaData>(temp.Count);
        foreach (var item in temp)
        {
            var map = ProfilePictureMetaDataMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ProfilePictureMetaDataContainer>> GetByIdAsContainerAsync(ProfilePictureId id,
        CancellationToken cancellationToken = default)
    {
        var result = await GetByIdAsync(id, cancellationToken);

        if (result.IsError)
        {
            return Result<ProfilePictureMetaDataContainer>.Error(result.Errors);
        }

        return Result<ProfilePictureMetaDataContainer>.Success(new ProfilePictureMetaDataContainer(id, result.Value!));
    }

    public async Task<Result> AddAsync(ProfilePictureMetaDataContainer coverImageMetaDataContainer,
        CancellationToken cancellationToken = default)
    {
        if (coverImageMetaDataContainer is null)
        {
            return Result.Error(DomainErrors.GeneralErrors.InputIsNull);
        }

        var exists = await ExistsAsync(coverImageMetaDataContainer.Id, cancellationToken);

        if (exists.IsError)
        {
            return exists;
        }

        if (exists.Value)
        {
            return Result.Success();
        }

        if (coverImageMetaDataContainer.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(coverImageMetaDataContainer);
        }

        await Context.ProfilePictures.AddRangeAsync(
            coverImageMetaDataContainer.AvailableImageMetaData.Select(ProfilePictureMetaDataMapper.Map),
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(ProfilePictureId coverImageId, CancellationToken cancellationToken = default)
    {
        await Context.ProfilePictures
            .Where(x => x.PublicId == coverImageId.Value)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }
}