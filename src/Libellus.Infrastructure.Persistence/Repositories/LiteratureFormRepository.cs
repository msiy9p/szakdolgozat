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
using DomainLiteratureForm = Libellus.Domain.Entities.LiteratureForm;
using PersistenceLiteratureForm = Libellus.Infrastructure.Persistence.DataModels.LiteratureForm;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class
    LiteratureFormRepository : BaseGroupedRepository<LiteratureFormRepository, PersistenceLiteratureForm>,
        ILiteratureFormRepository
{
    public LiteratureFormRepository(ApplicationContext context, ICurrentGroupService currentGroupService,
        ILogger<LiteratureFormRepository> logger) : base(context, currentGroupService, logger)
    {
    }

    internal LiteratureFormRepository(ApplicationContext context, GroupId currentGroupId, ILogger logger) : base(
        context, currentGroupId, logger)
    {
    }

    protected override IQueryable<PersistenceLiteratureForm> GetFiltered()
    {
        return Context.LiteratureForms
            .Where(x => x.GroupId == CurrentGroupId);
    }

    public async Task<Result<bool>> ExistsAsync(LiteratureFormId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainLiteratureForm>> GetByIdAsync(LiteratureFormId id,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return LiteratureFormErrors.LiteratureFormNotFound.ToErrorResult<DomainLiteratureForm>();
        }

        return LiteratureFormMapper.Map(found);
    }

    public async Task<Result<ICollection<DomainLiteratureForm>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainLiteratureForm>(found.Count);
        foreach (var item in found)
        {
            var map = LiteratureFormMapper.Map(item);

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

    public Result<UserId?> GetCreatorId(LiteratureFormId id)
    {
        var found = GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefault();

        return found.ToResult();
    }

    public async Task<Result<DomainLiteratureForm>> FindByNameAsync(ShortName name,
        CancellationToken cancellationToken = default)
    {
        if (name is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<DomainLiteratureForm>();
        }

        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.NameNormalized == name.ValueNormalized, cancellationToken);

        if (found is null)
        {
            return LiteratureFormErrors.LiteratureFormNotFound.ToErrorResult<DomainLiteratureForm>();
        }

        return LiteratureFormMapper.Map(found);
    }

    public async Task<Result<ICollection<DomainLiteratureForm>>> SearchAsync(SearchTerm searchTerm,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainLiteratureForm>>();
        }

        var found = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainLiteratureForm>(found.Count);
        foreach (var item in found)
        {
            var map = LiteratureFormMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<LiteratureFormId>> AddIfNotExistsAsync(DomainLiteratureForm entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);
        if (exists.IsError)
        {
            return Result<LiteratureFormId>.Error(exists.Errors);
        }

        if (exists.Value)
        {
            return entity.Id.ToResult();
        }

        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.NameNormalized == entity.Name.ValueNormalized, cancellationToken);

        if (found is not null)
        {
            return Result<LiteratureFormId>.Success(found.Id);
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = LiteratureFormMapper.Map(entity, CurrentGroupId);

        await Context.LiteratureForms.AddAsync(item, cancellationToken);

        return item.Id.ToResult();
    }

    public async Task<Result> UpdateAsync(DomainLiteratureForm entity, CancellationToken cancellationToken = default)
    {
        var result = await GetFiltered()
            .AnyAsync(x => x.NameNormalized == entity.Name.ValueNormalized && x.Id != entity.Id, cancellationToken);

        if (result)
        {
            return LiteratureFormErrors.LiteratureFormAlreadyExists.ToErrorResult();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = LiteratureFormMapper.Map(entity, CurrentGroupId);

        Context.LiteratureForms.Update(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(LiteratureFormId id, CancellationToken cancellationToken = default)
    {
        await Context.LiteratureForms.Where(x => x.GroupId == CurrentGroupId && x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(DomainLiteratureForm entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.LiteratureForms.Remove(LiteratureFormMapper.Map(entity, CurrentGroupId));

        return await Task.FromResult(Result.Success());
    }
}

internal static class LiteratureFormRepositoryHelper
{
    public static IQueryable<PersistenceLiteratureForm> ApplySortOrder(
        this IQueryable<PersistenceLiteratureForm> queryable, SortOrder sortOrder)
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

    public static IQueryable<PersistenceLiteratureForm> ApplyPagination(
        this IQueryable<PersistenceLiteratureForm> queryable, PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}