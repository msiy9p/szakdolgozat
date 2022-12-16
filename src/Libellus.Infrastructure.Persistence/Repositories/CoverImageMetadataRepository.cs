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

internal sealed class CoverImageMetadataRepository : BaseRepository<CoverImageMetadataRepository>,
    ICoverImageMetaDataRepository
{
    public CoverImageMetadataRepository(ApplicationContext context, ILogger<CoverImageMetadataRepository> logger) :
        base(context, logger)
    {
    }

    internal CoverImageMetadataRepository(ApplicationContext context, ILogger logger) : base(context, logger)
    {
    }

    public async Task<Result<bool>> ExistsAsync(CoverImageId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.CoverImages
            .AnyAsync(x => x.PublicId == id.Value, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<ICollection<CoverImageMetaData>>> GetByIdAsync(CoverImageId id,
        CancellationToken cancellationToken = default)
    {
        var temp = await Context.CoverImages
            .Where(x => x.PublicId == id.Value)
            .OrderBy(x => x.Width)
            .ThenBy(x => x.MimeType)
            .ToListAsync(cancellationToken);

        var output = new List<CoverImageMetaData>(temp.Count);
        foreach (var item in temp)
        {
            var map = CoverImageMetaDataMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<CoverImageMetaDataContainer>> GetByIdAsContainerAsync(CoverImageId id,
        CancellationToken cancellationToken = default)
    {
        var result = await GetByIdAsync(id, cancellationToken);

        if (result.IsError)
        {
            return Result<CoverImageMetaDataContainer>.Error(result.Errors);
        }

        return Result<CoverImageMetaDataContainer>.Success(new CoverImageMetaDataContainer(id, result.Value!));
    }

    public async Task<Result> AddAsync(IEnumerable<CoverImageMetaData> entities,
        CancellationToken cancellationToken = default)
    {
        await Context.CoverImages.AddRangeAsync(entities.Select(CoverImageMetaDataMapper.Map), cancellationToken);

        return Result.Success();
    }

    public async Task<Result> AddAsync(CoverImageMetaDataContainer coverImageMetaDataContainer,
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

        await Context.CoverImages.AddRangeAsync(
            coverImageMetaDataContainer.AvailableImageMetaData.Select(CoverImageMetaDataMapper.Map), cancellationToken);

        return Result.Success();
    }

    public async Task<Result> AddIfNotExistsAsync(CoverImageMetaDataContainer coverImageMetaDataContainer,
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

        return await AddAsync(coverImageMetaDataContainer, cancellationToken);
    }

    public async Task<Result> DeleteAsync(CoverImageId coverImageId, CancellationToken cancellationToken = default)
    {
        await Context.CoverImages
            .Where(x => x.PublicId == coverImageId.Value)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }
}