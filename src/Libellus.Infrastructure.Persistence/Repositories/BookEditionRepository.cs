using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types;
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
using DomainBookEdition = Libellus.Domain.Entities.BookEdition;
using DomainCoverImageMetaData = Libellus.Domain.Entities.CoverImageMetaData;
using DomainReading = Libellus.Domain.Entities.Reading;
using PersistenceBook = Libellus.Infrastructure.Persistence.DataModels.Book;
using PersistenceBookEdition = Libellus.Infrastructure.Persistence.DataModels.BookEdition;
using PersistenceLiteratureForm = Libellus.Infrastructure.Persistence.DataModels.LiteratureForm;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class BookEditionRepository : BaseGroupedRepository<BookEditionRepository, PersistenceBookEdition>,
    IBookEditionRepository
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public BookEditionRepository(ApplicationContext context, ICurrentGroupService currentGroupService,
        ILogger<BookEditionRepository> logger, IDateTimeProvider dateTimeProvider) : base(context, currentGroupService,
        logger)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    internal BookEditionRepository(ApplicationContext context, GroupId currentGroupId, ILogger logger,
        IDateTimeProvider dateTimeProvider) : base(context,
        currentGroupId, logger)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    protected override IQueryable<PersistenceBookEdition> GetFiltered()
    {
        return Context.BookEditions
            .Include(x => x.Format)
            .Include(x => x.Language)
            .Include(x => x.Publisher)
            .Where(x => x.GroupId == CurrentGroupId);
    }

    public async Task<Result<bool>> ExistsAsync(BookEditionId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.BookEditions
            .Where(x => x.GroupId == CurrentGroupId)
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainBookEdition>> GetByIdAsync(BookEditionId id,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return BookEditionErrors.BookEditionNotFound.ToErrorResult<DomainBookEdition>();
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

        IBookReadOnlyRepository bookRepository = new BookRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);
        var compact = await bookRepository.GetCompactVmByIdAsync(found.BookId, cancellationToken);
        if (compact.IsError)
        {
            return Result<DomainBookEdition>.Error(compact.Errors);
        }

        var map = BookEditionMapper.Map(found, compact.Value, userVm, foundCovers);
        if (map.IsError)
        {
            return map;
        }

        var offset = await GetReadingCountAsync(id, cancellationToken);
        if (offset.IsSuccess)
        {
            map.Value.SetReadingCountOffset(offset.Value);
        }

        return map;
    }

    public async Task<Result<ICollection<DomainBookEdition>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        IBookReadOnlyRepository bookRepository = new BookRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);

        var output = new List<DomainBookEdition>(found.Count);
        foreach (var item in found)
        {
            var compact = await bookRepository.GetCompactVmByIdAsync(item.BookId, cancellationToken);
            if (compact.IsError)
            {
                return Result<ICollection<DomainBookEdition>>.Error(compact.Errors);
            }

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

            var map = BookEditionMapper.Map(item, compact.Value, userVm, foundCovers);
            if (map.IsSuccess)
            {
                var offset = await GetReadingCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetReadingCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainBookEdition>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var count = await GetFiltered()
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        IBookReadOnlyRepository bookRepository = new BookRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);

        var output = new List<DomainBookEdition>(found.Count);
        foreach (var item in found)
        {
            var compact = await bookRepository.GetCompactVmByIdAsync(item.BookId, cancellationToken);
            if (compact.IsError)
            {
                return Result<PaginationDetail<ICollection<DomainBookEdition>>>.Error(compact.Errors);
            }

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

            var map = BookEditionMapper.Map(item, compact.Value, userVm, foundCovers);
            if (map.IsSuccess)
            {
                var offset = await GetReadingCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetReadingCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainBookEdition>>>.Success(
            new PaginationDetail<ICollection<DomainBookEdition>>(count, adjusted, output));
    }

    public async Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default)
    {
        var count = await Context.BookEditions
            .Where(x => x.GroupId == CurrentGroupId)
            .CountAsync(cancellationToken);

        return Result<int>.Success(count);
    }

    public async Task<Result<int>> GetReadingCountAsync(BookEditionId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return BookEditionErrors.BookEditionNotFound.ToErrorResult<int>();
        }

        var count = await Context.Readings
            .Where(x => x.BookEditionId == id)
            .CountAsync(cancellationToken);

        return Result<int>.Success(count);
    }

    public Result<UserId?> GetCreatorId(BookEditionId id)
    {
        var found = GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefault();

        return found.ToResult();
    }

    public async Task<Result<DomainBookEdition>> GetByIdWithReadingsAsync(BookEditionId id,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return BookEditionErrors.BookEditionNotFound.ToErrorResult<DomainBookEdition>();
        }

        IBookReadOnlyRepository bookRepository = new BookRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);

        var compact = await bookRepository.GetCompactVmByIdAsync(found.BookId, cancellationToken);
        if (compact.IsError)
        {
            return Result<DomainBookEdition>.Error(compact.Errors);
        }

        var foundCovers = new List<DomainCoverImageMetaData>();

        if (found.CoverImageId.HasValue)
        {
            foundCovers.AddRange(await GetCoversAsync(found.CoverImageId.Value, cancellationToken));
        }

        var readings = await Context.Readings
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.BookEditionId == id)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainReading>(readings.Count);
        foreach (var item in readings)
        {
            var compactVm = await GetCompactVmByIdAsync(item.BookEditionId, cancellationToken);
            if (compactVm.IsError)
            {
                return Result<DomainBookEdition>.Error(compactVm.Errors);
            }

            var scoreMultiplier = await Context.Books
                .Include(x => x.LiteratureForm)
                .Where(x => x.GroupId == CurrentGroupId)
                .Where(x => x.Id == found.BookId)
                .Select<PersistenceBook, decimal?>(x =>
                    x.LiteratureForm == null ? null : x.LiteratureForm.ScoreMultiplier)
                .FirstOrDefaultAsync(cancellationToken);

            var plf = scoreMultiplier.HasValue
                ? new PersistenceLiteratureForm() { ScoreMultiplier = scoreMultiplier.Value }
                : null;

            var creatorUserVm = await GetUserPicturedVmAsync(item.CreatorId, cancellationToken);
            if (creatorUserVm is null)
            {
                var userVm2 = await GetUserVmAsync(item.CreatorId, cancellationToken);
                var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);
                creatorUserVm = temp.IsSuccess ? temp.Value : null;
            }

            UserPicturedVm? noteUserVm = null;
            if (item.Note?.CreatorId is not null)
            {
                noteUserVm = await GetUserPicturedVmAsync(item.Note.CreatorId.Value, cancellationToken);

                if (noteUserVm is null)
                {
                    var userVm2 = await GetUserVmAsync(item.Note.CreatorId.Value, cancellationToken);
                    var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);
                    noteUserVm = temp.IsSuccess ? temp.Value : null;
                }
            }

            var map = ReadingMapper.Map(item, creatorUserVm!, noteUserVm, found, plf, compactVm.Value);
            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        UserVm? userVm1 = null;
        if (found.CreatorId.HasValue)
        {
            userVm1 = await GetUserVmAsync(found.CreatorId.Value, cancellationToken);
        }

        return BookEditionMapper.Map(found, compact.Value, userVm1, foundCovers, output);
    }

    public async Task<Result<BookEditionCompactVm>> GetCompactVmByIdAsync(BookEditionId id,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => new { x.Id, x.FriendlyId, x.Title, x.BookId })
            .FirstOrDefaultAsync(cancellationToken);

        if (found is null)
        {
            return BookEditionErrors.BookEditionNotFound.ToErrorResult<BookEditionCompactVm>();
        }

        var authors = await Context.BookAuthorConnectors
            .Include(x => x.Author)
            .Where(x => x.BookId == found.BookId)
            .Where(x => x.Author.GroupId == CurrentGroupId)
            .Select(x => new AuthorVm(x.Author.Id, AuthorFriendlyId.Convert(x.Author.FriendlyId).Value,
                Name.Create(x.Author.Name).Value))
            .ToListAsync(cancellationToken);

        var fid = BookEditionFriendlyId.Convert(found.FriendlyId);
        if (!fid.HasValue)
        {
            return GeneralErrors.StringNullOrWhiteSpace.ToErrorResult<BookEditionCompactVm>();
        }

        var title = Title.Create(found.Title);
        if (title.IsError)
        {
            return Result<BookEditionCompactVm>.Error(title.Errors);
        }

        var output = new BookEditionCompactVm(found.Id, fid.Value, title.Value, authors);

        return output.ToResult();
    }

    public async Task<Result<ICollection<DomainBookEdition>>> FindByIsbnAsync(Isbn isbn,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .Where(x => x.Isbn == isbn.Value)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        IBookReadOnlyRepository bookRepository = new BookRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);

        var output = new List<DomainBookEdition>(found.Count);
        foreach (var item in found)
        {
            var compact = await bookRepository.GetCompactVmByIdAsync(item.BookId, cancellationToken);
            if (compact.IsError)
            {
                return Result<ICollection<DomainBookEdition>>.Error(compact.Errors);
            }

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

            var map = BookEditionMapper.Map(item, compact, userVm, foundCovers);
            if (map.IsSuccess)
            {
                var offset = await GetReadingCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetReadingCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainBookEdition>>>> FindByIsbnAsync(Isbn isbn,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var count = await GetFiltered()
            .Where(x => x.Isbn == isbn.Value)
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .Where(x => x.Isbn == isbn.Value)
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        IBookReadOnlyRepository bookRepository = new BookRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);

        var output = new List<DomainBookEdition>(found.Count);
        foreach (var item in found)
        {
            var compact = await bookRepository.GetCompactVmByIdAsync(item.BookId, cancellationToken);
            if (compact.IsError)
            {
                return Result<PaginationDetail<ICollection<DomainBookEdition>>>.Error(compact.Errors);
            }

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

            var map = BookEditionMapper.Map(item, compact.Value, userVm, foundCovers);
            if (map.IsSuccess)
            {
                var offset = await GetReadingCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetReadingCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainBookEdition>>>.Success(
            new PaginationDetail<ICollection<DomainBookEdition>>(count, adjusted, output));
    }

    public async Task<Result<ICollection<DomainBookEdition>>> FindByTitleAsync(Title title,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (title is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainBookEdition>>();
        }

        var found = await GetFiltered()
            .Where(x => EF.Functions.ILike(x.TitleNormalized, $"%{title.ValueNormalized}%"))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        IBookReadOnlyRepository bookRepository = new BookRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);

        var output = new List<DomainBookEdition>(found.Count);
        foreach (var item in found)
        {
            var compact = await bookRepository.GetCompactVmByIdAsync(item.BookId, cancellationToken);
            if (compact.IsError)
            {
                return Result<ICollection<DomainBookEdition>>.Error(compact.Errors);
            }

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

            var map = BookEditionMapper.Map(item, compact.Value, userVm, foundCovers);
            if (map.IsSuccess)
            {
                var offset = await GetReadingCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetReadingCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainBookEdition>>>> FindByTitleAsync(Title title,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        if (title is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<PaginationDetail<ICollection<DomainBookEdition>>>();
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

        IBookReadOnlyRepository bookRepository = new BookRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);

        var output = new List<DomainBookEdition>(found.Count);
        foreach (var item in found)
        {
            var compact = await bookRepository.GetCompactVmByIdAsync(item.BookId, cancellationToken);
            if (compact.IsError)
            {
                return Result<PaginationDetail<ICollection<DomainBookEdition>>>.Error(compact.Errors);
            }

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

            var map = BookEditionMapper.Map(item, compact.Value, userVm, foundCovers);
            if (map.IsSuccess)
            {
                var offset = await GetReadingCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetReadingCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainBookEdition>>>.Success(
            new PaginationDetail<ICollection<DomainBookEdition>>(count, adjusted, output));
    }

    public async Task<Result<ICollection<DomainBookEdition>>> SearchAsync(SearchTerm searchTerm,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainBookEdition>>();
        }

        var found = await GetFiltered()
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        IBookReadOnlyRepository bookRepository = new BookRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);

        var output = new List<DomainBookEdition>(found.Count);
        foreach (var item in found)
        {
            var compact = await bookRepository.GetCompactVmByIdAsync(item.BookId, cancellationToken);
            if (compact.IsError)
            {
                return Result<ICollection<DomainBookEdition>>.Error(compact.Errors);
            }

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

            var map = BookEditionMapper.Map(item, compact.Value, userVm, foundCovers);
            if (map.IsSuccess)
            {
                var offset = await GetReadingCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetReadingCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainBookEdition>>>> SearchAsync(SearchTerm searchTerm,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<PaginationDetail<ICollection<DomainBookEdition>>>();
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

        IBookReadOnlyRepository bookRepository = new BookRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);

        var output = new List<DomainBookEdition>(found.Count);
        foreach (var item in found)
        {
            var compact = await bookRepository.GetCompactVmByIdAsync(item.BookId, cancellationToken);
            if (compact.IsError)
            {
                return Result<PaginationDetail<ICollection<DomainBookEdition>>>.Error(compact.Errors);
            }

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

            var map = BookEditionMapper.Map(item, compact.Value, userVm, foundCovers);
            if (map.IsSuccess)
            {
                var offset = await GetReadingCountAsync(item.Id, cancellationToken);
                if (offset.IsSuccess)
                {
                    map.Value.SetReadingCountOffset(offset.Value);
                }

                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainBookEdition>>>.Success(
            new PaginationDetail<ICollection<DomainBookEdition>>(count, adjusted, output));
    }

    public async Task<Result<BookEditionId>> AddIfNotExistsAsync(DomainBookEdition entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);

        if (exists.IsError)
        {
            return Result<BookEditionId>.Error(exists.Errors);
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
                return Result<BookEditionId>.Error(result.Errors);
            }
        }

        if (entity.Format is not null)
        {
            IFormatRepository formatRepository = new FormatRepository(Context, CurrentGroupId, Logger);
            var existResult = await formatRepository.AddIfNotExistsAsync(entity.Format, cancellationToken);
            if (existResult.IsError)
            {
                return Result<BookEditionId>.Error(existResult.Errors);
            }

            if (entity.Format.Id != existResult.Value)
            {
                var newFormat = await formatRepository.GetByIdAsync(existResult.Value, cancellationToken);
                if (newFormat.IsError)
                {
                    return Result<BookEditionId>.Error(newFormat.Errors);
                }

                entity.ChangeFormat(newFormat.Value, _dateTimeProvider);
            }
        }

        if (entity.Language is not null)
        {
            ILanguageRepository languageRepository = new LanguageRepository(Context, CurrentGroupId, Logger);
            var existResult = await languageRepository.AddIfNotExistsAsync(entity.Language, cancellationToken);
            if (existResult.IsError)
            {
                return Result<BookEditionId>.Error(existResult.Errors);
            }

            if (entity.Language.Id != existResult.Value)
            {
                var newLanguage = await languageRepository.GetByIdAsync(existResult.Value, cancellationToken);
                if (newLanguage.IsError)
                {
                    return Result<BookEditionId>.Error(newLanguage.Errors);
                }

                entity.ChangeLanguage(newLanguage.Value, _dateTimeProvider);
            }
        }

        if (entity.Publisher is not null)
        {
            IPublisherRepository publisherRepository = new PublisherRepository(Context, CurrentGroupId, Logger);
            var existResult = await publisherRepository.AddIfNotExistsAsync(entity.Publisher, cancellationToken);
            if (existResult.IsError)
            {
                return Result<BookEditionId>.Error(existResult.Errors);
            }

            if (entity.Publisher.Id != existResult.Value)
            {
                var newPublisher = await publisherRepository.GetByIdAsync(existResult.Value, cancellationToken);
                if (newPublisher.IsError)
                {
                    return Result<BookEditionId>.Error(newPublisher.Errors);
                }

                entity.ChangePublisher(newPublisher.Value, _dateTimeProvider);
            }
        }

        var item = BookEditionMapper.Map(entity, CurrentGroupId);

        await Context.BookEditions.AddAsync(item, cancellationToken);

        var changeTracker = entity.GetReadingTracker();
        if (!changeTracker.HasChanges)
        {
            return entity.Id.ToResult();
        }

        IReadingRepository readingRepository =
            new ReadingRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);
        foreach (var reading in changeTracker.GetItems())
        {
            var mapResult = await readingRepository.AddIfNotExistsAsync(reading, cancellationToken);
            if (mapResult.IsError)
            {
                return Result<BookEditionId>.Error(mapResult.Errors);
            }
        }

        return entity.Id.ToResult();
    }

    public async Task<Result> UpdateAsync(DomainBookEdition entity, CancellationToken cancellationToken = default)
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

        if (entity.Format is not null)
        {
            IFormatRepository formatRepository = new FormatRepository(Context, CurrentGroupId, Logger);
            var existResult = await formatRepository.AddIfNotExistsAsync(entity.Format, cancellationToken);
            if (existResult.IsError)
            {
                return existResult;
            }

            if (entity.Format.Id != existResult.Value)
            {
                var newItem = await formatRepository.GetByIdAsync(existResult.Value, cancellationToken);
                if (newItem.IsError)
                {
                    return newItem;
                }

                entity.ChangeFormat(newItem.Value, _dateTimeProvider);
            }
        }

        if (entity.Language is not null)
        {
            ILanguageRepository languageRepository = new LanguageRepository(Context, CurrentGroupId, Logger);
            var existResult = await languageRepository.AddIfNotExistsAsync(entity.Language, cancellationToken);
            if (existResult.IsError)
            {
                return existResult;
            }

            if (entity.Language.Id != existResult.Value)
            {
                var newItem = await languageRepository.GetByIdAsync(existResult.Value, cancellationToken);
                if (newItem.IsError)
                {
                    return newItem;
                }

                entity.ChangeLanguage(newItem.Value, _dateTimeProvider);
            }
        }

        if (entity.Publisher is not null)
        {
            IPublisherRepository publisherRepository = new PublisherRepository(Context, CurrentGroupId, Logger);
            var existResult = await publisherRepository.AddIfNotExistsAsync(entity.Publisher, cancellationToken);
            if (existResult.IsError)
            {
                return existResult;
            }

            if (entity.Publisher.Id != existResult.Value)
            {
                var newItem = await publisherRepository.GetByIdAsync(existResult.Value, cancellationToken);
                if (newItem.IsError)
                {
                    return newItem;
                }

                entity.ChangePublisher(newItem.Value, _dateTimeProvider);
            }
        }

        var item = BookEditionMapper.Map(entity, CurrentGroupId);

        Context.BookEditions.Update(item);

        var changeTracker = entity.GetReadingTracker();
        if (!changeTracker.HasChanges)
        {
            return Result.Success();
        }

        await Context.Readings
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => changeTracker.GetRemovedIds().Contains(x.Id))
            .ExecuteDeleteAsync(cancellationToken);

        IReadingRepository readingRepository =
            new ReadingRepository(Context, CurrentGroupId, Logger, _dateTimeProvider);
        foreach (var readingId in changeTracker.GetNewItemsChronologically())
        {
            var reading = changeTracker.GetById(readingId);
            var mapResult = await readingRepository.AddIfNotExistsAsync(reading!, cancellationToken);
            if (mapResult.IsError)
            {
                return mapResult;
            }
        }

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(BookEditionId id, CancellationToken cancellationToken = default)
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

        await Context.BookEditions.Where(x => x.GroupId == CurrentGroupId && x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(DomainBookEdition entity, CancellationToken cancellationToken = default)
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

        Context.BookEditions.Remove(BookEditionMapper.Map(entity, CurrentGroupId));

        return await Task.FromResult(Result.Success());
    }
}

internal static class BookEditionRepositoryHelper
{
    public static IQueryable<PersistenceBookEdition> ApplySortOrder(this IQueryable<PersistenceBookEdition> queryable,
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

    public static IQueryable<PersistenceBookEdition> ApplyPagination(this IQueryable<PersistenceBookEdition> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}