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
using DomainWarningTag = Libellus.Domain.Entities.WarningTag;
using PersistenceWarningTag = Libellus.Infrastructure.Persistence.DataModels.WarningTag;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class WarningTagRepository : BaseGroupedRepository<WarningTagRepository, PersistenceWarningTag>,
    IWarningTagRepository
{
    public WarningTagRepository(ApplicationContext context, ICurrentGroupService currentGroupService,
        ILogger<WarningTagRepository> logger) : base(context, currentGroupService, logger)
    {
    }

    internal WarningTagRepository(ApplicationContext context, GroupId currentGroupId, ILogger logger) : base(context,
        currentGroupId, logger)
    {
    }

    protected override IQueryable<PersistenceWarningTag> GetFiltered()
    {
        return Context.WarningTags
            .Where(x => x.GroupId == CurrentGroupId);
    }

    public async Task<Result<bool>> ExistsAsync(WarningTagId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainWarningTag>> GetByIdAsync(WarningTagId id,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return WarningTagErrors.WarningTagNotFound.ToErrorResult<DomainWarningTag>();
        }

        return WarningTagMapper.Map(found);
    }

    public async Task<Result<ICollection<DomainWarningTag>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainWarningTag>(found.Count);
        foreach (var item in found)
        {
            var map = WarningTagMapper.Map(item);

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

    public Result<UserId?> GetCreatorId(WarningTagId id)
    {
        var found = GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefault();

        return found.ToResult();
    }

    public async Task<Result<DomainWarningTag>> FindByNameAsync(ShortName name,
        CancellationToken cancellationToken = default)
    {
        if (name is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<DomainWarningTag>();
        }

        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.NameNormalized == name.ValueNormalized, cancellationToken);

        if (found is null)
        {
            return WarningTagErrors.WarningTagNotFound.ToErrorResult<DomainWarningTag>();
        }

        return WarningTagMapper.Map(found);
    }

    public async Task<Result<ICollection<DomainWarningTag>>> SearchAsync(SearchTerm searchTerm,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainWarningTag>>();
        }

        var found = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainWarningTag>(found.Count);
        foreach (var item in found)
        {
            var map = WarningTagMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<DomainWarningTag>>> GetAllByBookIdAsync(BookId bookId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var bookExists = await Context.Books
            .Where(x => x.GroupId == CurrentGroupId)
            .AnyAsync(x => x.Id == bookId, cancellationToken);

        if (!bookExists)
        {
            return BookErrors.BookNotFound.ToErrorResult<ICollection<DomainWarningTag>>();
        }

        var found = await Context.WarningTags
            .Include(x => x.BookWarningTagConnectors)
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.BookWarningTagConnectors.Any(y => y.BookId == bookId))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainWarningTag>(found.Count);
        foreach (var item in found)
        {
            var map = WarningTagMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<WarningTagId>> AddIfNotExistsAsync(DomainWarningTag entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);
        if (exists.IsError)
        {
            return Result<WarningTagId>.Error(exists.Errors);
        }

        if (exists.Value)
        {
            return entity.Id.ToResult();
        }

        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.NameNormalized == entity.Name.ValueNormalized, cancellationToken);

        if (found is not null)
        {
            return Result<WarningTagId>.Success(found.Id);
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = WarningTagMapper.Map(entity, CurrentGroupId);

        await Context.WarningTags.AddAsync(item, cancellationToken);

        return item.Id.ToResult();
    }

    public async Task<Result> UpdateAsync(DomainWarningTag entity, CancellationToken cancellationToken = default)
    {
        var result = await GetFiltered()
            .AnyAsync(x => x.NameNormalized == entity.Name.ValueNormalized && x.Id != entity.Id, cancellationToken);

        if (result)
        {
            return WarningTagErrors.WarningTagAlreadyExists.ToErrorResult();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = WarningTagMapper.Map(entity, CurrentGroupId);

        Context.WarningTags.Update(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(WarningTagId id, CancellationToken cancellationToken = default)
    {
        var item = new PersistenceWarningTag()
        {
            Id = id,
            GroupId = CurrentGroupId
        };

        Context.WarningTags.Remove(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(DomainWarningTag entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.WarningTags.Remove(WarningTagMapper.Map(entity, CurrentGroupId));

        return await Task.FromResult(Result.Success());
    }
}

internal static class WarningTagRepositoryHelper
{
    public static IQueryable<PersistenceWarningTag> ApplySortOrder(this IQueryable<PersistenceWarningTag> queryable,
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

    public static IQueryable<PersistenceWarningTag> ApplyPagination(this IQueryable<PersistenceWarningTag> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}