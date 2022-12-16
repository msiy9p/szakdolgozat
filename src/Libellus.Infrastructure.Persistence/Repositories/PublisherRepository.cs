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
using DomainPublisher = Libellus.Domain.Entities.Publisher;
using PersistencePublisher = Libellus.Infrastructure.Persistence.DataModels.Publisher;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class PublisherRepository : BaseGroupedRepository<PublisherRepository, PersistencePublisher>,
    IPublisherRepository
{
    public PublisherRepository(ApplicationContext context, ICurrentGroupService currentGroupService,
        ILogger<PublisherRepository> logger) : base(context, currentGroupService, logger)
    {
    }

    internal PublisherRepository(ApplicationContext context, GroupId currentGroupId, ILogger logger) : base(context,
        currentGroupId, logger)
    {
    }

    protected override IQueryable<PersistencePublisher> GetFiltered()
    {
        return Context.Publishers
            .Where(x => x.GroupId == CurrentGroupId);
    }

    public async Task<Result<bool>> ExistsAsync(PublisherId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainPublisher>> GetByIdAsync(PublisherId id,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return PublisherErrors.PublisherNotFound.ToErrorResult<DomainPublisher>();
        }

        return PublisherMapper.Map(found);
    }

    public async Task<Result<ICollection<DomainPublisher>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainPublisher>(found.Count);
        foreach (var item in found)
        {
            var map = PublisherMapper.Map(item);

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

    public Result<UserId?> GetCreatorId(PublisherId id)
    {
        var found = GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefault();

        return found.ToResult();
    }

    public async Task<Result<DomainPublisher>> FindByNameAsync(ShortName name,
        CancellationToken cancellationToken = default)
    {
        if (name is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<DomainPublisher>();
        }

        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.NameNormalized == name.ValueNormalized, cancellationToken);

        if (found is null)
        {
            return PublisherErrors.PublisherNotFound.ToErrorResult<DomainPublisher>();
        }

        return PublisherMapper.Map(found);
    }

    public async Task<Result<ICollection<DomainPublisher>>> SearchAsync(SearchTerm searchTerm,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainPublisher>>();
        }

        var found = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainPublisher>(found.Count);
        foreach (var item in found)
        {
            var map = PublisherMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PublisherId>> AddIfNotExistsAsync(DomainPublisher entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);
        if (exists.IsError)
        {
            return Result<PublisherId>.Error(exists.Errors);
        }

        if (exists.Value)
        {
            return entity.Id.ToResult();
        }

        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.NameNormalized == entity.Name.ValueNormalized, cancellationToken);

        if (found is not null)
        {
            return Result<PublisherId>.Success(found.Id);
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = PublisherMapper.Map(entity, CurrentGroupId);

        await Context.Publishers.AddAsync(item, cancellationToken);

        return item.Id.ToResult();
    }

    public async Task<Result> UpdateAsync(DomainPublisher entity, CancellationToken cancellationToken = default)
    {
        var result = await GetFiltered()
            .AnyAsync(x => x.NameNormalized == entity.Name.ValueNormalized && x.Id != entity.Id, cancellationToken);

        if (result)
        {
            return PublisherErrors.PublisherAlreadyExists.ToErrorResult();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = PublisherMapper.Map(entity, CurrentGroupId);

        Context.Publishers.Update(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(PublisherId id, CancellationToken cancellationToken = default)
    {
        var item = new PersistencePublisher()
        {
            Id = id,
            GroupId = CurrentGroupId
        };

        Context.Publishers.Remove(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(DomainPublisher entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.Publishers.Remove(PublisherMapper.Map(entity, CurrentGroupId));

        return await Task.FromResult(Result.Success());
    }
}

internal static class PublisherRepositoryHelper
{
    public static IQueryable<PersistencePublisher> ApplySortOrder(this IQueryable<PersistencePublisher> queryable,
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

    public static IQueryable<PersistencePublisher> ApplyPagination(this IQueryable<PersistencePublisher> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}