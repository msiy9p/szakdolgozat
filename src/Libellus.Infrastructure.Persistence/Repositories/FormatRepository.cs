using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Libellus.Infrastructure.Persistence.Mapping;
using Libellus.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Libellus.Domain.Errors.DomainErrors;
using DomainFormat = Libellus.Domain.Entities.Format;
using PersistenceFormat = Libellus.Infrastructure.Persistence.DataModels.Format;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class FormatRepository : BaseGroupedRepository<FormatRepository, PersistenceFormat>, IFormatRepository
{
    public FormatRepository(ApplicationContext context, ICurrentGroupService currentGroupService,
        ILogger<FormatRepository> logger) : base(context, currentGroupService, logger)
    {
    }

    internal FormatRepository(ApplicationContext context, GroupId currentGroupId, ILogger logger) : base(context,
        currentGroupId, logger)
    {
    }

    protected override IQueryable<PersistenceFormat> GetFiltered()
    {
        return Context.Formats
            .Where(x => x.GroupId == CurrentGroupId);
    }

    public async Task<Result<bool>> ExistsAsync(FormatId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainFormat>> GetByIdAsync(FormatId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return FormatErrors.FormatNotFound.ToErrorResult<DomainFormat>();
        }

        return FormatMapper.Map(found);
    }

    public async Task<Result<ICollection<DomainFormat>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainFormat>(found.Count);
        foreach (var item in found)
        {
            var map = FormatMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default)
    {
        var count = await GetFiltered()
            .CountAsync(cancellationToken);

        return Result<int>.Success(count);
    }

    public Result<UserId?> GetCreatorId(FormatId id)
    {
        var found = GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefault();

        return found.ToResult();
    }

    public async Task<Result<DomainFormat>> FindByNameAsync(ShortName name,
        CancellationToken cancellationToken = default)
    {
        if (name is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<DomainFormat>();
        }

        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.NameNormalized == name.ValueNormalized, cancellationToken);

        if (found is null)
        {
            return FormatErrors.FormatNotFound.ToErrorResult<DomainFormat>();
        }

        return FormatMapper.Map(found);
    }

    public async Task<Result<ICollection<DomainFormat>>> SearchAsync(SearchTerm searchTerm,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainFormat>>();
        }

        var found = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainFormat>(found.Count);
        foreach (var item in found)
        {
            var map = FormatMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<FormatId>> AddIfNotExistsAsync(DomainFormat entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);
        if (exists.IsError)
        {
            return Result<FormatId>.Error(exists.Errors);
        }

        if (exists.Value)
        {
            return entity.Id.ToResult();
        }

        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.NameNormalized == entity.Name.ValueNormalized, cancellationToken);

        if (found is not null)
        {
            return Result<FormatId>.Success(found.Id);
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = FormatMapper.Map(entity, CurrentGroupId);

        await Context.Formats.AddAsync(item, cancellationToken);

        return item.Id.ToResult();
    }

    public async Task<Result> UpdateAsync(DomainFormat entity, CancellationToken cancellationToken = default)
    {
        var result = await GetFiltered()
            .AnyAsync(x => x.NameNormalized == entity.Name.ValueNormalized && x.Id != entity.Id, cancellationToken);

        if (result)
        {
            return FormatErrors.FormatAlreadyExists.ToErrorResult();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = FormatMapper.Map(entity, CurrentGroupId);

        Context.Formats.Update(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(FormatId id, CancellationToken cancellationToken = default)
    {
        var item = new PersistenceFormat()
        {
            Id = id,
            GroupId = CurrentGroupId
        };

        Context.Formats.Remove(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(DomainFormat entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.Formats.Remove(FormatMapper.Map(entity, CurrentGroupId));

        return await Task.FromResult(Result.Success());
    }
}

internal static class FormatRepositoryHelper
{
    public static IQueryable<PersistenceFormat> ApplySortOrder(this IQueryable<PersistenceFormat> queryable,
        SortOrder sortOrder)
    {
        switch (sortOrder)
        {
            case SortOrder.Ascending:
                return queryable
                    .OrderBy(x => x.NameNormalized)
                    .ThenBy(x => x.CreatedOnUtc);

            case SortOrder.Descending:
                return queryable
                    .OrderByDescending(x => x.NameNormalized)
                    .ThenByDescending(x => x.CreatedOnUtc);

            default:
                goto case SortOrder.Ascending;
        }
    }

    public static IQueryable<PersistenceFormat> ApplyPagination(this IQueryable<PersistenceFormat> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}