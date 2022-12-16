using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Libellus.Infrastructure.Persistence.Mapping;
using Libellus.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Libellus.Domain.Errors.DomainErrors;
using DomainAuthor = Libellus.Domain.Entities.Author;
using DomainCoverImageMetaData = Libellus.Domain.Entities.CoverImageMetaData;
using PersistenceAuthor = Libellus.Infrastructure.Persistence.DataModels.Author;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class AuthorRepository : BaseGroupedRepository<AuthorRepository, PersistenceAuthor>, IAuthorRepository
{
    public AuthorRepository(ApplicationContext context, ICurrentGroupService currentGroupService,
        ILogger<AuthorRepository> logger) : base(context, currentGroupService, logger)
    {
    }

    internal AuthorRepository(ApplicationContext context, GroupId currentGroupId, ILogger logger) : base(context,
        currentGroupId, logger)
    {
    }

    protected override IQueryable<PersistenceAuthor> GetFiltered()
    {
        return Context.Authors
            .Where(x => x.GroupId == CurrentGroupId);
    }

    public async Task<Result<bool>> ExistsAsync(AuthorId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainAuthor>> GetByIdAsync(AuthorId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return AuthorErrors.AuthorNotFound.ToErrorResult<DomainAuthor>();
        }

        var foundCovers = new List<DomainCoverImageMetaData>();

        if (found.CoverImageId.HasValue)
        {
            foundCovers.AddRange(await GetCoversAsync(found.CoverImageId.Value, cancellationToken));
        }

        UserVm? userVm = null;
        if (found.CreatorId.HasValue)
        {
            userVm = await GetUserVmAsync(found.CreatorId.Value, cancellationToken);
        }

        return AuthorMapper.Map(found, userVm, foundCovers);
    }

    public async Task<Result<ICollection<DomainAuthor>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainAuthor>(found.Count);
        foreach (var item in found)
        {
            var foundCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = AuthorMapper.Map(item, userVm, foundCovers);
            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainAuthor>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var count = await GetFiltered()
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainAuthor>(found.Count);
        foreach (var item in found)
        {
            var foundCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = AuthorMapper.Map(item, userVm, foundCovers);
            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainAuthor>>>.Success(
            new PaginationDetail<ICollection<DomainAuthor>>(count, adjusted, output));
    }

    public async Task<Result<ICollection<DomainAuthor>>> GetAllByBookIdAsync(BookId bookId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var bookExists = await Context.Books
            .Where(x => x.GroupId == CurrentGroupId)
            .AnyAsync(x => x.Id == bookId, cancellationToken);

        if (!bookExists)
        {
            return BookErrors.BookNotFound.ToErrorResult<ICollection<DomainAuthor>>();
        }

        var found = await Context.Authors
            .Include(x => x.BookAuthorConnectors)
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.BookAuthorConnectors.Any(y => y.BookId == bookId))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainAuthor>(found.Count);
        foreach (var item in found)
        {
            var foundCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = AuthorMapper.Map(item, userVm, foundCovers);
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

    public Result<UserId?> GetCreatorId(AuthorId id)
    {
        var found = GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefault();

        return found.ToResult();
    }

    public async Task<Result<ICollection<DomainAuthor>>> FindByNameAsync(Name name,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (name is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainAuthor>>();
        }

        var found = await GetFiltered()
            .Where(x => EF.Functions.ILike(x.NameNormalized, $"%{name.ValueNormalized}%"))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainAuthor>(found.Count);
        foreach (var item in found)
        {
            var foundCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = AuthorMapper.Map(item, userVm, foundCovers);
            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainAuthor>>>> FindByNameAsync(Name name,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        if (name is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<PaginationDetail<ICollection<DomainAuthor>>>();
        }

        var count = await GetFiltered()
            .Where(x => EF.Functions.ILike(x.NameNormalized, $"%{name.ValueNormalized}%"))
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .Where(x => EF.Functions.ILike(x.NameNormalized, $"%{name.ValueNormalized}%"))
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainAuthor>(found.Count);
        foreach (var item in found)
        {
            var foundCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = AuthorMapper.Map(item, userVm, foundCovers);
            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainAuthor>>>.Success(
            new PaginationDetail<ICollection<DomainAuthor>>(count, adjusted, output));
    }

    public async Task<Result<ICollection<DomainAuthor>>> SearchAsync(SearchTerm searchTerm,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainAuthor>>();
        }

        var found = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized)
                        || x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainAuthor>(found.Count);
        foreach (var item in found)
        {
            var foundCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = AuthorMapper.Map(item, userVm, foundCovers);
            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainAuthor>>>> SearchAsync(SearchTerm searchTerm,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<PaginationDetail<ICollection<DomainAuthor>>>();
        }

        var count = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized)
                        || x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized)
                        || x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainAuthor>(found.Count);
        foreach (var item in found)
        {
            var foundCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = AuthorMapper.Map(item, userVm, foundCovers);
            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainAuthor>>>.Success(
            new PaginationDetail<ICollection<DomainAuthor>>(count, adjusted, output));
    }

    public async Task<Result<AuthorId>> AddIfNotExistsAsync(DomainAuthor entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);

        if (exists.IsError)
        {
            return Result<AuthorId>.Error(exists.Errors);
        }

        if (exists.Value)
        {
            return entity.Id.ToResult();
        }

        var item = AuthorMapper.Map(entity, CurrentGroupId);

        if (entity.AvailableCovers is not null)
        {
            var result = await AddCoversAsync(entity.AvailableCovers!, cancellationToken);

            if (result.IsError)
            {
                return Result<AuthorId>.Error(result.Errors);
            }
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        await Context.Authors.AddAsync(item, cancellationToken);

        return entity.Id.ToResult();
    }

    public async Task<Result> UpdateAsync(DomainAuthor entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = AuthorMapper.Map(entity, CurrentGroupId);

        var oldCoverImageId = await GetFiltered()
            .Where(x => x.Id == item.Id)
            .Select(x => x.CoverImageId)
            .FirstOrDefaultAsync(cancellationToken);

        if (oldCoverImageId is not null && item.CoverImageId.HasValue && oldCoverImageId != item.CoverImageId.Value)
        {
            var result = await DeleteCoversAsync(oldCoverImageId.Value, cancellationToken);

            if (result.IsError)
            {
                return result;
            }
        }

        if (entity.AvailableCovers is not null)
        {
            var result = await AddCoversAsync(entity.AvailableCovers!, cancellationToken);

            if (result.IsError)
            {
                return result;
            }
        }

        Context.Authors.Update(item);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(AuthorId id, CancellationToken cancellationToken = default)
    {
        var oldCoverImageId = await GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CoverImageId)
            .FirstOrDefaultAsync(cancellationToken);

        if (oldCoverImageId is not null)
        {
            var result = await DeleteCoversAsync(oldCoverImageId.Value, cancellationToken);

            if (result.IsError)
            {
                return result;
            }
        }

        await Context.Authors.Where(x => x.GroupId == CurrentGroupId && x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(DomainAuthor entity, CancellationToken cancellationToken = default)
    {
        if (entity.AvailableCovers is not null)
        {
            var result = await DeleteCoversAsync(entity.AvailableCovers.Id, cancellationToken);

            if (result.IsError)
            {
                return result;
            }
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.Authors.Remove(AuthorMapper.Map(entity, CurrentGroupId));

        return await Task.FromResult(Result.Success());
    }
}

internal static class AuthorRepositoryHelper
{
    public static IQueryable<PersistenceAuthor> ApplySortOrder(this IQueryable<PersistenceAuthor> queryable,
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

    public static IQueryable<PersistenceAuthor> ApplyPagination(this IQueryable<PersistenceAuthor> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}