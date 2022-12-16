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
using DomainGenre = Libellus.Domain.Entities.Genre;
using PersistenceGenre = Libellus.Infrastructure.Persistence.DataModels.Genre;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class GenreRepository : BaseGroupedRepository<GenreRepository, PersistenceGenre>, IGenreRepository
{
    public GenreRepository(ApplicationContext context, ICurrentGroupService currentGroupService,
        ILogger<GenreRepository> logger) : base(context, currentGroupService, logger)
    {
    }

    internal GenreRepository(ApplicationContext context, GroupId currentGroupId, ILogger logger) : base(context,
        currentGroupId, logger)
    {
    }

    protected override IQueryable<PersistenceGenre> GetFiltered()
    {
        return Context.Genres
            .Where(x => x.GroupId == CurrentGroupId);
    }

    public async Task<Result<bool>> ExistsAsync(GenreId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainGenre>> GetByIdAsync(GenreId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return GenreErrors.GenreNotFound.ToErrorResult<DomainGenre>();
        }

        return GenreMapper.Map(found);
    }

    public async Task<Result<ICollection<DomainGenre>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainGenre>(found.Count);
        foreach (var item in found)
        {
            var map = GenreMapper.Map(item);

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

    public Result<UserId?> GetCreatorId(GenreId id)
    {
        var found = GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefault();

        return found.ToResult();
    }

    public async Task<Result<DomainGenre>> FindByNameAsync(ShortName name,
        CancellationToken cancellationToken = default)
    {
        if (name is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<DomainGenre>();
        }

        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.NameNormalized == name.ValueNormalized, cancellationToken);

        if (found is null)
        {
            return GenreErrors.GenreNotFound.ToErrorResult<DomainGenre>();
        }

        return GenreMapper.Map(found);
    }

    public async Task<Result<ICollection<DomainGenre>>> SearchAsync(SearchTerm searchTerm,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainGenre>>();
        }

        var found = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainGenre>(found.Count);
        foreach (var item in found)
        {
            var map = GenreMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<DomainGenre>>> GetAllByFictionAsync(bool isFiction = true,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .Where(x => x.IsFiction == isFiction)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainGenre>(found.Count);
        foreach (var item in found)
        {
            var map = GenreMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<DomainGenre>>> GetAllByBookIdAsync(BookId bookId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var bookExists = await Context.Books
            .Where(x => x.GroupId == CurrentGroupId)
            .AnyAsync(x => x.Id == bookId, cancellationToken);

        if (!bookExists)
        {
            return BookErrors.BookNotFound.ToErrorResult<ICollection<DomainGenre>>();
        }

        var found = await Context.Genres
            .Include(x => x.BookGenreConnectors)
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.BookGenreConnectors.Any(y => y.BookId == bookId))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainGenre>(found.Count);
        foreach (var item in found)
        {
            var map = GenreMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<GenreId>> AddIfNotExistsAsync(DomainGenre entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);
        if (exists.IsError)
        {
            return Result<GenreId>.Error(exists.Errors);
        }

        if (exists.Value)
        {
            return entity.Id.ToResult();
        }

        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.NameNormalized == entity.Name.ValueNormalized, cancellationToken);

        if (found is not null)
        {
            return Result<GenreId>.Success(found.Id);
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = GenreMapper.Map(entity, CurrentGroupId);

        await Context.Genres.AddAsync(item, cancellationToken);

        return item.Id.ToResult();
    }

    public async Task<Result> UpdateAsync(DomainGenre entity, CancellationToken cancellationToken = default)
    {
        var result = await GetFiltered()
            .AnyAsync(x => x.NameNormalized == entity.Name.ValueNormalized && x.Id != entity.Id, cancellationToken);

        if (result)
        {
            return GenreErrors.GenreAlreadyExists.ToErrorResult();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = GenreMapper.Map(entity, CurrentGroupId);

        Context.Genres.Update(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(GenreId id, CancellationToken cancellationToken = default)
    {
        var item = new PersistenceGenre()
        {
            Id = id,
            GroupId = CurrentGroupId
        };

        Context.Genres.Remove(item);

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(DomainGenre entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.Genres.Remove(GenreMapper.Map(entity, CurrentGroupId));

        return await Task.FromResult(Result.Success());
    }
}

internal static class GenreRepositoryHelper
{
    public static IQueryable<PersistenceGenre> ApplySortOrder(this IQueryable<PersistenceGenre> queryable,
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

    public static IQueryable<PersistenceGenre> ApplyPagination(this IQueryable<PersistenceGenre> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}