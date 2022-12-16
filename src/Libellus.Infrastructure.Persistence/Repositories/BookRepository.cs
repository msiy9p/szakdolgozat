using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using Libellus.Infrastructure.Persistence.DataModels;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Libellus.Infrastructure.Persistence.Mapping;
using Libellus.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Libellus.Domain.Errors.DomainErrors;
using DomainAuthor = Libellus.Domain.Entities.Author;
using DomainBook = Libellus.Domain.Entities.Book;
using DomainBookEdition = Libellus.Domain.Entities.BookEdition;
using DomainCoverImageMetaData = Libellus.Domain.Entities.CoverImageMetaData;
using DomainGenre = Libellus.Domain.Entities.Genre;
using DomainTag = Libellus.Domain.Entities.Tag;
using DomainWarningTag = Libellus.Domain.Entities.WarningTag;
using PersistenceBook = Libellus.Infrastructure.Persistence.DataModels.Book;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class BookRepository : BaseGroupedRepository<BookRepository, PersistenceBook>, IBookRepository
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public BookRepository(ApplicationContext context, ICurrentGroupService currentGroupService,
        ILogger<BookRepository> logger, IDateTimeProvider dateTimeProvider) : base(context, currentGroupService, logger)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    internal BookRepository(ApplicationContext context, GroupId groupId, ILogger logger,
        IDateTimeProvider dateTimeProvider) : base(context, groupId,
        logger)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    protected override IQueryable<PersistenceBook> GetFiltered()
    {
        return Context.Books
            .Include(x => x.LiteratureForm)
            .Include(x => x.BookSeriesConnector)
#pragma warning disable CS8602
            .ThenInclude(x => x.Series)
#pragma warning restore CS8602
            .Where(x => x.GroupId == CurrentGroupId);
    }

    private async Task<ICollection<DomainAuthor>> GetRelatedAuthorsAsync(BookId id,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        IAuthorReadOnlyRepository authorRepository = new AuthorRepository(Context, CurrentGroupId, Logger);
        var results = await authorRepository.GetAllByBookIdAsync(id, sortOrder, cancellationToken);

        if (results.IsError)
        {
            return Array.Empty<DomainAuthor>();
        }

        return results.Value!;
    }

    private async Task<ICollection<DomainGenre>> GetRelatedGenresAsync(BookId id,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        IGenreReadOnlyRepository genreRepository = new GenreRepository(Context, CurrentGroupId, Logger);
        var results = await genreRepository.GetAllByBookIdAsync(id, sortOrder, cancellationToken);

        if (results.IsError)
        {
            return Array.Empty<DomainGenre>();
        }

        return results.Value!;
    }

    private async Task<ICollection<DomainTag>> GetRelatedTagsAsync(BookId id, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        ITagReadOnlyRepository tagRepository = new TagRepository(Context, CurrentGroupId, Logger);
        var results = await tagRepository.GetAllByBookIdAsync(id, sortOrder, cancellationToken);

        if (results.IsError)
        {
            return Array.Empty<DomainTag>();
        }

        return results.Value!;
    }

    private async Task<ICollection<DomainWarningTag>> GetRelatedWarningTagsAsync(BookId id,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        IWarningTagReadOnlyRepository warningTagRepository = new WarningTagRepository(Context, CurrentGroupId, Logger);
        var results = await warningTagRepository.GetAllByBookIdAsync(id, sortOrder, cancellationToken);
        if (results.IsError)
        {
            return Array.Empty<DomainWarningTag>();
        }

        return results.Value!;
    }

    public async Task<Result<bool>> ExistsAsync(BookId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.Books
            .Where(x => x.GroupId == CurrentGroupId)
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainBook>> GetByIdAsync(BookId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return BookErrors.BookNotFound.ToErrorResult<DomainBook>();
        }

        var foundCovers = new List<DomainCoverImageMetaData>();

        if (found.CoverImageId.HasValue)
        {
            foundCovers.AddRange(await GetCoversAsync(found.CoverImageId.Value, cancellationToken));
        }

        var authorsTask = await GetRelatedAuthorsAsync(id, SortOrder.Ascending, cancellationToken);
        var genresTask = await GetRelatedGenresAsync(id, SortOrder.Ascending, cancellationToken);
        var tagsTask = await GetRelatedTagsAsync(id, SortOrder.Ascending, cancellationToken);
        var warningTagsTask = await GetRelatedWarningTagsAsync(id, SortOrder.Ascending, cancellationToken);

        UserVm? userVm = null;
        if (found.CreatorId.HasValue)
        {
            userVm = await GetUserVmAsync(found.CreatorId.Value, cancellationToken);
        }

        var map = BookMapper.Map(found, userVm, null, foundCovers, authorsTask, genresTask, tagsTask, warningTagsTask);
        if (map.IsError)
        {
            return map;
        }

        var offset = await GetBookEditionCountAsync(id, cancellationToken);
        if (offset.IsSuccess)
        {
            map.Value.SetBookEditionCountOffset(offset.Value);
        }

        return map;
    }

    public async Task<Result<ICollection<DomainBook>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainBook>(found.Count);
        foreach (var item in found)
        {
            var foundCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            var authorsTask = await GetRelatedAuthorsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var genresTask = await GetRelatedGenresAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var tagsTask = await GetRelatedTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var warningTagsTask = await GetRelatedWarningTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = BookMapper.Map(item, userVm, null, foundCovers, authorsTask, genresTask,
                tagsTask, warningTagsTask);

            if (map.IsSuccess)
            {
                var offset = await GetBookEditionCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetBookEditionCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainBook>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var count = await GetFiltered()
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainBook>(found.Count);
        foreach (var item in found)
        {
            var foundCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            var authorsTask = await GetRelatedAuthorsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var genresTask = await GetRelatedGenresAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var tagsTask = await GetRelatedTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var warningTagsTask = await GetRelatedWarningTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = BookMapper.Map(item, userVm, null, foundCovers, authorsTask, genresTask,
                tagsTask, warningTagsTask);

            if (map.IsSuccess)
            {
                var offset = await GetBookEditionCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetBookEditionCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainBook>>>.Success(
            new PaginationDetail<ICollection<DomainBook>>(count, adjusted, output));
    }

    public async Task<Result<ICollection<DomainBook>>> GetAllByAuthorIdAsync(AuthorId authorId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var authorExists = await Context.Authors
            .Where(x => x.GroupId == CurrentGroupId)
            .AnyAsync(x => x.Id == authorId, cancellationToken);

        if (!authorExists)
        {
            return AuthorErrors.AuthorNotFound.ToErrorResult<ICollection<DomainBook>>();
        }

        var found = await GetFiltered()
            .Where(x => x.BookAuthorConnectors
                .Any(y => y.AuthorId == authorId))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainBook>(found.Count);
        foreach (var item in found)
        {
            var foundCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            var authorsTask = await GetRelatedAuthorsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var genresTask = await GetRelatedGenresAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var tagsTask = await GetRelatedTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var warningTagsTask = await GetRelatedWarningTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = BookMapper.Map(item, userVm, null, foundCovers, authorsTask, genresTask,
                tagsTask, warningTagsTask);

            if (map.IsSuccess)
            {
                var offset = await GetBookEditionCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetBookEditionCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainBook>>>> GetAllByAuthorIdAsync(AuthorId authorId,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var authorExists = await Context.Authors
            .Where(x => x.GroupId == CurrentGroupId)
            .AnyAsync(x => x.Id == authorId, cancellationToken);

        if (!authorExists)
        {
            return AuthorErrors.AuthorNotFound.ToErrorResult<PaginationDetail<ICollection<DomainBook>>>();
        }

        var count = await GetFiltered()
            .Where(x => x.BookAuthorConnectors
                .Any(y => y.AuthorId == authorId))
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .Where(x => x.BookAuthorConnectors
                .Any(y => y.AuthorId == authorId))
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainBook>(found.Count);
        foreach (var item in found)
        {
            var foundCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            var authorsTask = await GetRelatedAuthorsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var genresTask = await GetRelatedGenresAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var tagsTask = await GetRelatedTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var warningTagsTask = await GetRelatedWarningTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = BookMapper.Map(item, userVm, null, foundCovers, authorsTask, genresTask,
                tagsTask, warningTagsTask);

            if (map.IsSuccess)
            {
                var offset = await GetBookEditionCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetBookEditionCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainBook>>>.Success(
            new PaginationDetail<ICollection<DomainBook>>(count, adjusted, output));
    }

    public async Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default)
    {
        var count = await Context.Books
            .Where(x => x.GroupId == CurrentGroupId)
            .CountAsync(cancellationToken);

        return Result<int>.Success(count);
    }

    public async Task<Result<int>> GetBookEditionCountAsync(BookId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return BookErrors.BookNotFound.ToErrorResult<int>();
        }

        var count = await Context.BookEditions
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.BookId == id)
            .CountAsync(cancellationToken);

        return Result<int>.Success(count);
    }

    public Result<UserId?> GetCreatorId(BookId id)
    {
        var found = GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefault();

        return found.ToResult();
    }

    public async Task<Result<DomainBook>> GetByIdWithBookEditionsAsync(BookId id,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return BookErrors.BookNotFound.ToErrorResult<DomainBook>();
        }

        var compact = await GetCompactVmByIdAsync(id, cancellationToken);
        if (compact.IsError)
        {
            return Result<DomainBook>.Error(compact.Errors);
        }

        var foundCovers = new List<DomainCoverImageMetaData>();
        if (found.CoverImageId.HasValue)
        {
            foundCovers.AddRange(await GetCoversAsync(found.CoverImageId.Value, cancellationToken));
        }

        var foundBookEditionsTask = await Context.BookEditions
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.BookId == found.Id)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var authorsTask = await GetRelatedAuthorsAsync(id, SortOrder.Ascending, cancellationToken);
        var genresTask = await GetRelatedGenresAsync(id, SortOrder.Ascending, cancellationToken);
        var tagsTask = await GetRelatedTagsAsync(id, SortOrder.Ascending, cancellationToken);
        var warningTagsTask = await GetRelatedWarningTagsAsync(id, SortOrder.Ascending, cancellationToken);

        var bookEditions = new List<DomainBookEdition>(foundBookEditionsTask.Count);
        foreach (var item in foundBookEditionsTask)
        {
            var foundBookEditionCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundBookEditionCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var mapResult = BookEditionMapper.Map(item, compact.Value, userVm, foundBookEditionCovers);
            if (mapResult.IsSuccess)
            {
                bookEditions.Add(mapResult.Value!);
            }
        }

        UserVm? userVm1 = null;
        if (found.CreatorId.HasValue)
        {
            userVm1 = await GetUserVmAsync(found.CreatorId.Value, cancellationToken);
        }

        return BookMapper.Map(found, userVm1, null, foundCovers, authorsTask, genresTask, tagsTask,
            warningTagsTask, bookEditions);
    }

    public async Task<Result<BookCompactVm>> GetCompactVmByIdAsync(BookId id,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => new { x.Id, x.FriendlyId, x.Title })
            .FirstOrDefaultAsync(cancellationToken);

        if (found is null)
        {
            return BookErrors.BookNotFound.ToErrorResult<BookCompactVm>();
        }

        var authors = await Context.BookAuthorConnectors
            .Include(x => x.Author)
            .Where(x => x.BookId == found.Id)
            .Where(x => x.Author.GroupId == CurrentGroupId)
            .Select(x => new AuthorVm(x.Author.Id, AuthorFriendlyId.Convert(x.Author.FriendlyId).Value,
                Name.Create(x.Author.Name).Value))
            .ToListAsync(cancellationToken);

        var fid = BookFriendlyId.Convert(found.FriendlyId);
        if (!fid.HasValue)
        {
            return GeneralErrors.StringNullOrWhiteSpace.ToErrorResult<BookCompactVm>();
        }

        var title = Title.Create(found.Title);
        if (title.IsError)
        {
            return Result<BookCompactVm>.Error(title.Errors);
        }

        var output = new BookCompactVm(found.Id, fid.Value, title.Value, authors);

        return output.ToResult();
    }

    public async Task<Result<ICollection<BookCompactVm>>> GetAllCompactVmByShelfIdAsync(ShelfId shelfId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var found = await Context.Shelves
            .Where(x => x.GroupId == CurrentGroupId)
            .AnyAsync(x => x.Id == shelfId, cancellationToken);

        if (!found)
        {
            return Result<ICollection<BookCompactVm>>.Error(ShelfErrors.ShelfNotFound);
        }

        var books = await Context.ShelfBookConnectors
            .Include(x => x.Book)
            .Where(x => x.ShelfId == shelfId)
            .Where(x => x.Book.GroupId == CurrentGroupId)
            .Select(x => new { x.BookId, x.Book.FriendlyId, x.Book.Title })
            .ToListAsync(cancellationToken);

        var output = new List<BookCompactVm>(books.Count);
        foreach (var book in books)
        {
            var fid = BookFriendlyId.Convert(book.FriendlyId);
            if (!fid.HasValue)
            {
                continue;
            }

            var title = Title.Create(book.Title);
            if (title.IsError)
            {
                continue;
            }

            var authors = await Context.BookAuthorConnectors
                .Include(x => x.Author)
                .Where(x => x.BookId == book.BookId)
                .Where(x => x.Author.GroupId == CurrentGroupId)
                .Select(x => new AuthorVm(x.Author.Id, AuthorFriendlyId.Convert(x.Author.FriendlyId).Value,
                    Name.Create(x.Author.Name).Value))
                .ToListAsync(cancellationToken);

            output.Add(new BookCompactVm(book.BookId, fid.Value, title.Value, authors));
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<BookCompactVm>>> GetAllCompactVmBySeriesIdAsync(SeriesId seriesId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var found = await Context.Series
            .Where(x => x.GroupId == CurrentGroupId)
            .AnyAsync(x => x.Id == seriesId, cancellationToken);

        if (!found)
        {
            return Result<ICollection<BookCompactVm>>.Error(ShelfErrors.ShelfNotFound);
        }

        var books = await Context.BookSeriesConnectors
            .Include(x => x.Book)
            .Where(x => x.SeriesId == seriesId)
            .Where(x => x.Book.GroupId == CurrentGroupId)
            .Select(x => new { x.BookId, x.Book.FriendlyId, x.Book.Title })
            .ToListAsync(cancellationToken);

        var output = new List<BookCompactVm>(books.Count);
        foreach (var book in books)
        {
            var fid = BookFriendlyId.Convert(book.FriendlyId);
            if (!fid.HasValue)
            {
                continue;
            }

            var title = Title.Create(book.Title);
            if (title.IsError)
            {
                continue;
            }

            var authors = await Context.BookAuthorConnectors
                .Include(x => x.Author)
                .Where(x => x.BookId == book.BookId)
                .Where(x => x.Author.GroupId == CurrentGroupId)
                .Select(x => new AuthorVm(x.Author.Id, AuthorFriendlyId.Convert(x.Author.FriendlyId).Value,
                    Name.Create(x.Author.Name).Value))
                .ToListAsync(cancellationToken);

            output.Add(new BookCompactVm(book.BookId, fid.Value, title.Value, authors));
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<BookCompactVm>>> GetAllCompactVmByAuthorIdAsync(AuthorId authorId,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var found = await Context.Authors
            .Where(x => x.GroupId == CurrentGroupId)
            .AnyAsync(x => x.Id == authorId, cancellationToken);

        if (!found)
        {
            return Result<ICollection<BookCompactVm>>.Error(ShelfErrors.ShelfNotFound);
        }

        var books = await Context.BookAuthorConnectors
            .Include(x => x.Book)
            .Where(x => x.AuthorId == authorId)
            .Where(x => x.Book.GroupId == CurrentGroupId)
            .Select(x => new { x.BookId, x.Book.FriendlyId, x.Book.Title })
            .ToListAsync(cancellationToken);

        var output = new List<BookCompactVm>(books.Count);
        foreach (var book in books)
        {
            var fid = BookFriendlyId.Convert(book.FriendlyId);
            if (!fid.HasValue)
            {
                continue;
            }

            var title = Title.Create(book.Title);
            if (title.IsError)
            {
                continue;
            }

            var authors = await Context.BookAuthorConnectors
                .Include(x => x.Author)
                .Where(x => x.BookId == book.BookId)
                .Where(x => x.Author.GroupId == CurrentGroupId)
                .Select(x => new AuthorVm(x.Author.Id, AuthorFriendlyId.Convert(x.Author.FriendlyId).Value,
                    Name.Create(x.Author.Name).Value))
                .ToListAsync(cancellationToken);

            output.Add(new BookCompactVm(book.BookId, fid.Value, title.Value, authors));
        }

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<DomainBook>>> FindByTitleAsync(Title title,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (title is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainBook>>();
        }

        var found = await GetFiltered()
            .Where(x => EF.Functions.ILike(x.TitleNormalized, $"%{title.ValueNormalized}%"))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainBook>(found.Count);
        foreach (var item in found)
        {
            var foundCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            var authorsTask = await GetRelatedAuthorsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var genresTask = await GetRelatedGenresAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var tagsTask = await GetRelatedTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var warningTagsTask = await GetRelatedWarningTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = BookMapper.Map(item, userVm, null, foundCovers, authorsTask, genresTask,
                tagsTask, warningTagsTask);

            if (map.IsSuccess)
            {
                var offset = await GetBookEditionCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetBookEditionCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainBook>>>> FindByTitleAsync(Title title,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        if (title is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<PaginationDetail<ICollection<DomainBook>>>();
        }

        var count = await GetFiltered()
            .Where(x => EF.Functions.ILike(x.TitleNormalized, $"%{title.ValueNormalized}%"))
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .Where(x => EF.Functions.ILike(x.TitleNormalized, $"%{title.ValueNormalized}%"))
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainBook>(found.Count);
        foreach (var item in found)
        {
            var foundCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            var authorsTask = await GetRelatedAuthorsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var genresTask = await GetRelatedGenresAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var tagsTask = await GetRelatedTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var warningTagsTask = await GetRelatedWarningTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = BookMapper.Map(item, userVm, null, foundCovers, authorsTask, genresTask,
                tagsTask, warningTagsTask);

            if (map.IsSuccess)
            {
                var offset = await GetBookEditionCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetBookEditionCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainBook>>>.Success(
            new PaginationDetail<ICollection<DomainBook>>(count, adjusted, output));
    }

    public async Task<Result<ICollection<DomainBook>>> SearchAsync(SearchTerm searchTerm,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainBook>>();
        }

        var found = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainBook>(found.Count);
        foreach (var item in found)
        {
            var foundCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            var authorsTask = await GetRelatedAuthorsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var genresTask = await GetRelatedGenresAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var tagsTask = await GetRelatedTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var warningTagsTask = await GetRelatedWarningTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = BookMapper.Map(item, userVm, null, foundCovers, authorsTask, genresTask,
                tagsTask, warningTagsTask);

            if (map.IsSuccess)
            {
                var offset = await GetBookEditionCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetBookEditionCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainBook>>>> SearchAsync(SearchTerm searchTerm,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<PaginationDetail<ICollection<DomainBook>>>();
        }

        var count = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainBook>(found.Count);
        foreach (var item in found)
        {
            var foundCovers = new List<DomainCoverImageMetaData>();

            if (item.CoverImageId.HasValue)
            {
                foundCovers.AddRange(await GetCoversAsync(item.CoverImageId.Value, cancellationToken));
            }

            var authorsTask = await GetRelatedAuthorsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var genresTask = await GetRelatedGenresAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var tagsTask = await GetRelatedTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);
            var warningTagsTask = await GetRelatedWarningTagsAsync(item.Id, SortOrder.Ascending, cancellationToken);

            UserVm? userVm = null;
            if (item.CreatorId.HasValue)
            {
                userVm = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
            }

            var map = BookMapper.Map(item, userVm, null, foundCovers, authorsTask, genresTask,
                tagsTask, warningTagsTask);

            if (map.IsSuccess)
            {
                var offset = await GetBookEditionCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetBookEditionCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainBook>>>.Success(
            new PaginationDetail<ICollection<DomainBook>>(count, adjusted, output));
    }

    public async Task<Result<BookId>> AddIfNotExistsAsync(DomainBook entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);

        if (exists.IsError)
        {
            return Result<BookId>.Error(exists.Errors);
        }

        if (exists.Value)
        {
            return entity.Id.ToResult();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        if (entity.AvailableCovers is not null)
        {
            var result = await AddCoversAsync(entity.AvailableCovers!, cancellationToken);

            if (result.IsError)
            {
                return Result<BookId>.Error(result.Errors);
            }
        }

        if (entity.LiteratureForm is not null)
        {
            ILiteratureFormRepository literatureFormRepository =
                new LiteratureFormRepository(Context, CurrentGroupId, Logger);
            var existResult =
                await literatureFormRepository.AddIfNotExistsAsync(entity.LiteratureForm, cancellationToken);
            if (existResult.IsError)
            {
                return Result<BookId>.Error(existResult.Errors);
            }

            if (entity.LiteratureForm.Id != existResult.Value)
            {
                var litForm = await literatureFormRepository.GetByIdAsync(existResult.Value, cancellationToken);
                if (litForm.IsError)
                {
                    return Result<BookId>.Error(litForm.Errors);
                }

                entity.ChangeLiteratureForm(litForm.Value, _dateTimeProvider);
            }
        }

        if (entity.Series is not null && entity.NumberInSeries.HasValue)
        {
            ISeriesRepository seriesRepository =
                new SeriesRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);

            var existsResult = await seriesRepository.ExistsAsync(entity.Series.Id, cancellationToken);
            if (existsResult.IsError)
            {
                return Result<BookId>.Error(existsResult.Errors);
            }

            if (existsResult.Value)
            {
                var conFound = await Context.BookSeriesConnectors
                    .Where(x => x.BookId == entity.Id && x.SeriesId == entity.Series.Id)
                    .AnyAsync(x => x.NumberInSeries == entity.NumberInSeries.Value, cancellationToken);

                if (conFound)
                {
                    var max = await Context.BookSeriesConnectors
                        .Where(x => x.BookId == entity.Id && x.SeriesId == entity.Series.Id)
                        .MaxAsync(x => x.NumberInSeries, cancellationToken);

                    var bsc = new BookSeriesConnector(entity.Id, entity.Series.Id, max + 0.05m);
                    await Context.BookSeriesConnectors.AddAsync(bsc, cancellationToken);
                }
                else
                {
                    var bsc = new BookSeriesConnector(entity.Id, entity.Series.Id, entity.NumberInSeries.Value);
                    await Context.BookSeriesConnectors.AddAsync(bsc, cancellationToken);
                }
            }
            else
            {
                var seriesTitleIds = await Context.Series
                    .Where(x => x.GroupId == CurrentGroupId)
                    .Where(x => EF.Functions.ILike(x.TitleNormalized, $"%{entity.Series.Title.ValueNormalized}%"))
                    .Select(x => x.Id)
                    .Take(2)
                    .ToListAsync(cancellationToken);

                if (seriesTitleIds.Count == 1) // Only one series matches given title
                {
                    var connectorCount = await Context.BookSeriesConnectors
                        .Where(x => x.SeriesId == seriesTitleIds[0])
                        .Where(x => x.NumberInSeries == entity.NumberInSeries.Value)
                        .CountAsync(cancellationToken);

                    if (connectorCount == 0) // No connector exists for this series with matching number
                    {
                        var nis = entity.NumberInSeries.Value;
                        var newSeries = await seriesRepository.GetByIdAsync(seriesTitleIds[0], cancellationToken);
                        if (newSeries.IsError)
                        {
                            return Result<BookId>.Error(newSeries.Errors);
                        }

                        entity.ChangeSeries(newSeries.Value, _dateTimeProvider);
                        entity.ChangeNumberInSeries(nis, _dateTimeProvider);
                    }
                }
                else // multiple IDs found
                {
                    var existResult = await seriesRepository.AddIfNotExistsAsync(entity.Series, cancellationToken);
                    if (existResult.IsError)
                    {
                        return Result<BookId>.Error(existResult.Errors);
                    }
                }

                var bsc = new BookSeriesConnector(entity.Id, entity.Series.Id, entity.NumberInSeries.Value);
                await Context.BookSeriesConnectors.AddAsync(bsc, cancellationToken);
            }
        }

        var authorTracker = entity.GetAuthorTracker();
        if (authorTracker.HasChanges)
        {
            IAuthorRepository authorRepository = new AuthorRepository(Context, CurrentGroupId, Logger);

            var corrected = new List<AuthorId>();
            var removed = new List<AuthorId>();
            foreach (var author in authorTracker.GetItems())
            {
                var existsResult = await authorRepository.ExistsAsync(author.Id, cancellationToken);
                if (existsResult.IsError)
                {
                    return Result<BookId>.Error(existsResult.Errors);
                }

                if (existsResult.Value)
                {
                    var conExists = await Context.BookAuthorConnectors
                        .AnyAsync(x => x.BookId == entity.Id && x.AuthorId == author.Id, cancellationToken);

                    if (!conExists)
                    {
                        var connector = new BookAuthorConnector(entity.Id, author.Id);
                        await Context.BookAuthorConnectors.AddAsync(connector, cancellationToken);
                    }
                }
                else
                {
                    var authorIds = await Context.Authors
                        .Where(x => x.GroupId == CurrentGroupId)
                        .Where(x => EF.Functions.ILike(x.NameNormalized, $"%{author.Name.ValueNormalized}%"))
                        .Select(x => x.Id)
                        .Take(2)
                        .ToListAsync(cancellationToken);

                    if (authorIds.Count == 1)
                    {
                        removed.Add(author.Id);
                        corrected.Add(authorIds[0]);
                    }
                    else
                    {
                        var result = await authorRepository.AddIfNotExistsAsync(author, cancellationToken);
                        if (result.IsError)
                        {
                            return Result<BookId>.Error(result.Errors);
                        }

                        var connector = new BookAuthorConnector(entity.Id, author.Id);
                        await Context.BookAuthorConnectors.AddAsync(connector, cancellationToken);
                    }
                }
            }

            foreach (var id in removed)
            {
                entity.RemoveAuthorById(id, _dateTimeProvider);
            }

            foreach (var id in corrected)
            {
                var temp = await authorRepository.GetByIdAsync(id, cancellationToken);
                if (temp.IsError)
                {
                    return Result<BookId>.Error(temp.Errors);
                }

                entity.AddAuthor(temp.Value, _dateTimeProvider);

                var connector = new BookAuthorConnector(entity.Id, id);
                await Context.BookAuthorConnectors.AddAsync(connector, cancellationToken);
            }
        }

        var genreTracker = entity.GetGenreTracker();
        if (genreTracker.HasChanges)
        {
            IGenreRepository genreRepository = new GenreRepository(Context, CurrentGroupId, Logger);
            var corrected = new List<GenreId>();
            var removed = new List<GenreId>();
            foreach (var genre in genreTracker.GetItems())
            {
                var result = await genreRepository.AddIfNotExistsAsync(genre, cancellationToken);
                if (result.IsError)
                {
                    return Result<BookId>.Error(result.Errors);
                }

                if (genre.Id != result.Value)
                {
                    removed.Add(genre.Id);
                    corrected.Add(result.Value);

                    var connector = new BookGenreConnector(entity.Id, result.Value);
                    await Context.BookGenreConnectors.AddAsync(connector, cancellationToken);
                }
                else
                {
                    var connector = new BookGenreConnector(entity.Id, genre.Id);
                    await Context.BookGenreConnectors.AddAsync(connector, cancellationToken);
                }
            }

            foreach (var genreId in removed)
            {
                entity.RemoveGenreById(genreId, _dateTimeProvider);
            }

            foreach (var genreId in corrected)
            {
                var newGenre = await genreRepository.GetByIdAsync(genreId, cancellationToken);
                if (newGenre.IsError)
                {
                    return Result<BookId>.Error(newGenre.Errors);
                }

                entity.AddGenre(newGenre.Value, _dateTimeProvider);
            }
        }

        var tagTracker = entity.GetTagTracker();
        if (tagTracker.HasChanges)
        {
            ITagRepository tagRepository = new TagRepository(Context, CurrentGroupId, Logger);
            var corrected = new List<TagId>();
            var removed = new List<TagId>();
            foreach (var tag in tagTracker.GetItems())
            {
                var result = await tagRepository.AddIfNotExistsAsync(tag, cancellationToken);
                if (result.IsError)
                {
                    return Result<BookId>.Error(result.Errors);
                }

                if (tag.Id != result.Value)
                {
                    removed.Add(tag.Id);
                    corrected.Add(result.Value);

                    var connector = new BookTagConnector(entity.Id, result.Value);
                    await Context.BookTagConnectors.AddAsync(connector, cancellationToken);
                }
                else
                {
                    var connector = new BookTagConnector(entity.Id, tag.Id);
                    await Context.BookTagConnectors.AddAsync(connector, cancellationToken);
                }
            }

            foreach (var tagId in removed)
            {
                entity.RemoveTagById(tagId, _dateTimeProvider);
            }

            foreach (var tagId in corrected)
            {
                var newTag = await tagRepository.GetByIdAsync(tagId, cancellationToken);
                if (newTag.IsError)
                {
                    return Result<BookId>.Error(newTag.Errors);
                }

                entity.AddTag(newTag.Value, _dateTimeProvider);
            }
        }

        var warningTagTracker = entity.GetWarningTagTracker();
        if (warningTagTracker.HasChanges)
        {
            IWarningTagRepository warningTagRepository = new WarningTagRepository(Context, CurrentGroupId, Logger);
            var corrected = new List<WarningTagId>();
            var removed = new List<WarningTagId>();
            foreach (var warningTag in warningTagTracker.GetItems())
            {
                var result = await warningTagRepository.AddIfNotExistsAsync(warningTag, cancellationToken);
                if (result.IsError)
                {
                    return Result<BookId>.Error(result.Errors);
                }

                if (warningTag.Id != result.Value)
                {
                    removed.Add(warningTag.Id);
                    corrected.Add(result.Value);

                    var connector = new BookWarningTagConnector(entity.Id, result.Value);
                    await Context.BookWarningTagConnectors.AddAsync(connector, cancellationToken);
                }
                else
                {
                    var connector = new BookWarningTagConnector(entity.Id, warningTag.Id);
                    await Context.BookWarningTagConnectors.AddAsync(connector, cancellationToken);
                }
            }

            foreach (var tagId in removed)
            {
                entity.RemoveWarningTagById(tagId, _dateTimeProvider);
            }

            foreach (var tagId in corrected)
            {
                var newTag = await warningTagRepository.GetByIdAsync(tagId, cancellationToken);
                if (newTag.IsError)
                {
                    return Result<BookId>.Error(newTag.Errors);
                }

                entity.AddWarningTag(newTag.Value, _dateTimeProvider);
            }
        }

        var item = BookMapper.Map(entity, CurrentGroupId);

        await Context.Books.AddAsync(item, cancellationToken);

        return entity.Id.ToResult();
    }

    public async Task<Result> UpdateAsync(DomainBook entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var oldCoverImageId = await GetFiltered()
            .Where(x => x.Id == entity.Id)
            .Select(x => x.CoverImageId)
            .FirstOrDefaultAsync(cancellationToken);

        if (oldCoverImageId is not null && entity.AvailableCovers is not null &&
            oldCoverImageId != entity.AvailableCovers.Id)
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

        if (entity.LiteratureForm is not null)
        {
            ILiteratureFormRepository literatureFormRepository =
                new LiteratureFormRepository(Context, CurrentGroupId, Logger);
            var existResult =
                await literatureFormRepository.AddIfNotExistsAsync(entity.LiteratureForm, cancellationToken);
            if (existResult.IsError)
            {
                return existResult;
            }

            if (entity.LiteratureForm.Id != existResult.Value)
            {
                var newItem = await literatureFormRepository.GetByIdAsync(existResult.Value, cancellationToken);
                if (newItem.IsError)
                {
                    return newItem;
                }

                entity.ChangeLiteratureForm(newItem.Value, _dateTimeProvider);
            }
        }

        var oldBookSeriesConnector = await GetFiltered()
            .Where(x => x.Id == entity.Id)
            .Select(x => x.BookSeriesConnector)
            .FirstOrDefaultAsync(cancellationToken);

        ISeriesRepository seriesRepository = new SeriesRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);
        if (oldBookSeriesConnector is null) // No prev. connector
        {
            if (entity.Series is not null && entity.NumberInSeries.HasValue)
            {
                var existResult = await seriesRepository.AddIfNotExistsAsync(entity.Series, cancellationToken);
                if (existResult.IsError)
                {
                    return existResult;
                }

                var bsc = new BookSeriesConnector(entity.Id, existResult.Value, entity.NumberInSeries.Value);
                await Context.BookSeriesConnectors.AddAsync(bsc, cancellationToken);
            }
        }
        else // Prev. connector exists
        {
            if (entity.Series is not null && entity.NumberInSeries.HasValue) // Connector exists
            {
                if (entity.Series.Id == oldBookSeriesConnector.SeriesId) // Series matches
                {
                    if (!decimal.Equals(entity.NumberInSeries.Value,
                            oldBookSeriesConnector.NumberInSeries)) // Number changed
                    {
                        var bsc = new BookSeriesConnector(entity.Id, entity.Series.Id, entity.NumberInSeries.Value);
                        Context.BookSeriesConnectors.Update(bsc);
                    }
                }
                else // new series
                {
                    var bsc = new BookSeriesConnector(entity.Id, entity.Series.Id, entity.NumberInSeries.Value);
                    await Context.BookSeriesConnectors.AddAsync(bsc, cancellationToken);
                }
            }
            else // Connector removed
            {
                Context.BookSeriesConnectors.Remove(oldBookSeriesConnector);
            }
        }

        var authorTracker = entity.GetAuthorTracker();
        if (authorTracker.HasChanges)
        {
            await Context.BookAuthorConnectors
                .Where(x => x.BookId == entity.Id)
                .Where(x => authorTracker.GetRemovedIds().Contains(x.AuthorId))
                .ExecuteDeleteAsync(cancellationToken);

            IAuthorRepository authorRepository = new AuthorRepository(Context, CurrentGroupId, Logger);
            foreach (var authorId in authorTracker.GetNewItemsChronologically())
            {
                var author = authorTracker.GetById(authorId);
                var result = await authorRepository.AddIfNotExistsAsync(author!, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }

                var connector = new BookAuthorConnector(entity.Id, authorId);
                await Context.BookAuthorConnectors.AddAsync(connector, cancellationToken);
            }
        }

        var genreTracker = entity.GetGenreTracker();
        if (genreTracker.HasChanges)
        {
            await Context.BookGenreConnectors
                .Where(x => x.BookId == entity.Id)
                .Where(x => genreTracker.GetRemovedIds().Contains(x.GenreId))
                .ExecuteDeleteAsync(cancellationToken);

            IGenreRepository genreRepository = new GenreRepository(Context, CurrentGroupId, Logger);
            var corrected = new List<GenreId>();
            var removed = new List<GenreId>();
            foreach (var genreId in genreTracker.GetNewItemsChronologically())
            {
                var genre = genreTracker.GetById(genreId);
                var result = await genreRepository.AddIfNotExistsAsync(genre!, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }

                if (genreId != result.Value)
                {
                    corrected.Add(result.Value);
                    removed.Add(genreId);
                }
                else
                {
                    var connector = new BookGenreConnector(entity.Id, genreId);
                    await Context.BookGenreConnectors.AddAsync(connector, cancellationToken);
                }
            }

            foreach (var genreId in removed)
            {
                entity.RemoveGenreById(genreId, _dateTimeProvider);
            }

            foreach (var genre in corrected)
            {
                var newGenre = await genreRepository.GetByIdAsync(genre, cancellationToken);
                if (newGenre.IsError)
                {
                    return newGenre;
                }

                entity.AddGenre(newGenre.Value, _dateTimeProvider);

                var connector = new BookGenreConnector(entity.Id, newGenre.Value.Id);
                await Context.BookGenreConnectors.AddAsync(connector, cancellationToken);
            }
        }

        var tagTracker = entity.GetTagTracker();
        if (tagTracker.HasChanges)
        {
            await Context.BookTagConnectors
                .Where(x => x.BookId == entity.Id)
                .Where(x => tagTracker.GetRemovedIds().Contains(x.TagId))
                .ExecuteDeleteAsync(cancellationToken);

            ITagRepository tagRepository = new TagRepository(Context, CurrentGroupId, Logger);
            var corrected = new List<TagId>();
            var removed = new List<TagId>();
            foreach (var tagId in tagTracker.GetNewItemsChronologically())
            {
                var tag = tagTracker.GetById(tagId);
                var result = await tagRepository.AddIfNotExistsAsync(tag!, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }

                if (tagId != result.Value)
                {
                    corrected.Add(result.Value);
                    removed.Add(tagId);
                }
                else
                {
                    var connector = new BookTagConnector(entity.Id, tagId);
                    await Context.BookTagConnectors.AddAsync(connector, cancellationToken);
                }
            }

            foreach (var tagId in removed)
            {
                entity.RemoveTagById(tagId, _dateTimeProvider);
            }

            foreach (var tag in corrected)
            {
                var newTag = await tagRepository.GetByIdAsync(tag, cancellationToken);
                if (newTag.IsError)
                {
                    return newTag;
                }

                entity.AddTag(newTag.Value, _dateTimeProvider);

                var connector = new BookTagConnector(entity.Id, newTag.Value.Id);
                await Context.BookTagConnectors.AddAsync(connector, cancellationToken);
            }
        }

        var warningTagTracker = entity.GetWarningTagTracker();
        if (warningTagTracker.HasChanges)
        {
            await Context.BookWarningTagConnectors
                .Where(x => x.BookId == entity.Id)
                .Where(x => warningTagTracker.GetRemovedIds().Contains(x.WarningTagId))
                .ExecuteDeleteAsync(cancellationToken);

            IWarningTagRepository warningTagRepository = new WarningTagRepository(Context, CurrentGroupId, Logger);
            var corrected = new List<WarningTagId>();
            var removed = new List<WarningTagId>();
            foreach (var warningTagId in warningTagTracker.GetNewItemsChronologically())
            {
                var warningTag = warningTagTracker.GetById(warningTagId);
                var result = await warningTagRepository.AddIfNotExistsAsync(warningTag!, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }

                if (warningTagId != result.Value)
                {
                    corrected.Add(result.Value);
                    removed.Add(warningTagId);
                }
                else
                {
                    var connector = new BookWarningTagConnector(entity.Id, warningTagId);
                    await Context.BookWarningTagConnectors.AddAsync(connector, cancellationToken);
                }
            }

            foreach (var warningTagId in removed)
            {
                entity.RemoveWarningTagById(warningTagId, _dateTimeProvider);
            }

            foreach (var warningTag in corrected)
            {
                var newWarningTag = await warningTagRepository.GetByIdAsync(warningTag, cancellationToken);
                if (newWarningTag.IsError)
                {
                    return newWarningTag;
                }

                entity.AddWarningTag(newWarningTag.Value, _dateTimeProvider);

                var connector = new BookWarningTagConnector(entity.Id, newWarningTag.Value.Id);
                await Context.BookWarningTagConnectors.AddAsync(connector, cancellationToken);
            }
        }

        var item = BookMapper.Map(entity, CurrentGroupId);

        Context.Books.Update(item);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(BookId id, CancellationToken cancellationToken = default)
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

        await Context.Books.Where(x => x.GroupId == CurrentGroupId && x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(DomainBook entity, CancellationToken cancellationToken = default)
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

        Context.Books.Remove(BookMapper.Map(entity, CurrentGroupId));

        return await Task.FromResult(Result.Success());
    }
}

internal static class BookRepositoryHelper
{
    public static IQueryable<PersistenceBook> ApplySortOrder(this IQueryable<PersistenceBook> queryable,
        SortOrder sortOrder)
    {
        switch (sortOrder)
        {
            case SortOrder.Ascending:
                return queryable
                    .OrderBy(x => x.TitleNormalized)
                    .ThenBy(x => x.CreatedOnUtc);

            case SortOrder.Descending:
                return queryable
                    .OrderByDescending(x => x.TitleNormalized)
                    .ThenByDescending(x => x.CreatedOnUtc);

            default:
                goto case SortOrder.Ascending;
        }
    }

    public static IQueryable<PersistenceBook> ApplyPagination(this IQueryable<PersistenceBook> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}