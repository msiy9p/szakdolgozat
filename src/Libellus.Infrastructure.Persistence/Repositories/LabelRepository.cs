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
using DomainLabel = Libellus.Domain.Entities.Label;
using PersistenceLabel = Libellus.Infrastructure.Persistence.DataModels.Label;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class LabelRepository : BaseGroupedRepository<LabelRepository, PersistenceLabel>, ILabelRepository
{
    public LabelRepository(ApplicationContext context, ICurrentGroupService currentGroupService,
        ILogger<LabelRepository> logger) : base(context, currentGroupService, logger)
    {
    }

    internal LabelRepository(ApplicationContext context, GroupId currentGroupId, ILogger logger) : base(context,
        currentGroupId, logger)
    {
    }

    protected override IQueryable<PersistenceLabel> GetFiltered()
    {
        return Context.Labels
            .Where(x => x.GroupId == CurrentGroupId);
    }

    public async Task<Result<bool>> ExistsAsync(LabelId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainLabel>> GetByIdAsync(LabelId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return LabelErrors.LabelNotFound.ToErrorResult<DomainLabel>();
        }

        return LabelMapper.Map(found);
    }

    public async Task<Result<ICollection<DomainLabel>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainLabel>(found.Count);
        foreach (var item in found)
        {
            var map = LabelMapper.Map(item);

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

    public async Task<Result<DomainLabel>> FindByNameAsync(ShortName name,
        CancellationToken cancellationToken = default)
    {
        if (name is null)
        {
            return GeneralErrors.StringNullOrWhiteSpace.ToErrorResult<DomainLabel>();
        }

        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.NameNormalized == name.ValueNormalized, cancellationToken);

        if (found is null)
        {
            return LabelErrors.LabelNotFound.ToErrorResult<DomainLabel>();
        }

        return LabelMapper.Map(found);
    }

    public async Task<Result<ICollection<DomainLabel>>> SearchAsync(SearchTerm searchTerm,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainLabel>>();
        }

        var found = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainLabel>(found.Count);
        foreach (var item in found)
        {
            var map = LabelMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<LabelId>> AddIfNotExistsAsync(DomainLabel entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);
        if (exists.IsError)
        {
            return Result<LabelId>.Error(exists.Errors);
        }

        if (exists.Value)
        {
            return entity.Id.ToResult();
        }

        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.NameNormalized == entity.Name.ValueNormalized, cancellationToken);

        if (found is not null)
        {
            return Result<LabelId>.Success(found.Id);
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = LabelMapper.Map(entity, CurrentGroupId);

        await Context.Labels.AddAsync(item, cancellationToken);

        return item.Id.ToResult();
    }

    public async Task<Result> UpdateAsync(DomainLabel entity, CancellationToken cancellationToken = default)
    {
        var result = await GetFiltered()
            .AnyAsync(x => x.NameNormalized == entity.Name.ValueNormalized && x.Id != entity.Id, cancellationToken);

        if (result)
        {
            return LabelErrors.LabelAlreadyExists.ToErrorResult();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = LabelMapper.Map(entity, CurrentGroupId);

        Context.Labels.Update(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(LabelId id, CancellationToken cancellationToken = default)
    {
        var item = new PersistenceLabel()
        {
            Id = id,
            GroupId = CurrentGroupId
        };

        Context.Labels.Remove(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(DomainLabel entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.Labels.Remove(LabelMapper.Map(entity, CurrentGroupId));

        return await Task.FromResult(Result.Success());
    }
}

internal static class LabelRepositoryHelper
{
    public static IQueryable<PersistenceLabel> ApplySortOrder(this IQueryable<PersistenceLabel> queryable,
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

    public static IQueryable<PersistenceLabel> ApplyPagination(this IQueryable<PersistenceLabel> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}