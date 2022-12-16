using Libellus.Application.Commands.Books.CreateBook;
using Libellus.Application.Commands.Books.DeleteBook;
using Libellus.Application.Commands.Books.DeleteBookById;
using Libellus.Application.Commands.Books.DeleteBookCoverImageById;
using Libellus.Application.Commands.Books.UpdateBook;
using Libellus.Application.Commands.Books.UpdateBookById;
using Libellus.Application.Commands.Books.UpdateBookCoverImageById;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Commands.Books;

public sealed class BookCommandHandler :
    ICommandHandler<CreateBookCommand, BookIds>,
    ICommandHandler<DeleteBookCommand>,
    ICommandHandler<DeleteBookByIdCommand>,
    ICommandHandler<UpdateBookCommand>,
    ICommandHandler<DeleteBookCoverImageByIdCommand>,
    ICommandHandler<UpdateBookCoverImageByIdCommand>,
    ICommandHandler<UpdateBookByIdCommand>
{
    private readonly IBookRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICoverImageRepository _coverImageRepository;

    public BookCommandHandler(IBookRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork, ICoverImageRepository coverImageRepository)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _coverImageRepository = coverImageRepository;
    }

    public async Task<Result<BookIds>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var dateTime = _dateTimeProvider.UtcNow;
        var id = BookId.Create();
        var fid = BookFriendlyId.Create();

        var item = Book.Create(id, dateTime, dateTime, fid, (UserVm?)userId, request.Title, request.DescriptionText,
            request.CoverImageMetaDataContainer);

        if (item.IsError)
        {
            return await Task.FromResult(Result<BookIds>.Error(item.Errors));
        }

        if (request.LiteratureForm is not null)
        {
            var tempId = request.LiteratureForm.LiteratureFormId ?? LiteratureFormId.Create();

            var temp = LiteratureForm.Create(tempId, dateTime, dateTime, userId,
                request.LiteratureForm.ShortName, request.LiteratureForm.ScoreMultiplier);
            if (temp.IsError)
            {
                return Result<BookIds>.Error(temp.Errors);
            }

            item.Value.ChangeLiteratureForm(temp.Value, _dateTimeProvider);
        }

        if (request.SeriesDto is not null)
        {
            var tempId = request.SeriesDto.SeriesId ?? SeriesId.Create();

            var temp = Domain.Entities.Series.Create(tempId, dateTime, dateTime, SeriesFriendlyId.Create(),
                (UserVm?)userId, request.SeriesDto.Title);
            if (temp.IsError)
            {
                return Result<BookIds>.Error(temp.Errors);
            }

            item.Value.ChangeSeries(temp.Value, _dateTimeProvider);
            item.Value.ChangeNumberInSeries(request.SeriesDto.NumberInSeries, _dateTimeProvider);
        }

        foreach (var input in request.Authors)
        {
            var tempId = input.AuthorId ?? AuthorId.Create();

            var temp = Author.Create(tempId, dateTime, dateTime, AuthorFriendlyId.Create(),
                (UserVm?)userId, input.Name, null);
            if (temp.IsError)
            {
                return Result<BookIds>.Error(temp.Errors);
            }

            item.Value.AddAuthor(temp.Value, _dateTimeProvider);
        }

        foreach (var input in request.Genres)
        {
            var tempId = input.GenreId ?? GenreId.Create();

            var temp = Genre.Create(tempId, dateTime, dateTime, userId,
                input.ShortName, input.IsFiction);
            if (temp.IsError)
            {
                return Result<BookIds>.Error(temp.Errors);
            }

            item.Value.AddGenre(temp.Value, _dateTimeProvider);
        }

        foreach (var input in request.Tags)
        {
            var tempId = input.TagId ?? TagId.Create();

            var temp = Tag.Create(tempId, dateTime, dateTime, userId,
                input.Name);
            if (temp.IsError)
            {
                return Result<BookIds>.Error(temp.Errors);
            }

            item.Value.AddTag(temp.Value, _dateTimeProvider);
        }

        foreach (var input in request.WarningTags)
        {
            var tempId = input.WarningTagId ?? WarningTagId.Create();

            var temp = WarningTag.Create(tempId, dateTime, dateTime, userId,
                input.Name);
            if (temp.IsError)
            {
                return Result<BookIds>.Error(temp.Errors);
            }

            item.Value.AddWarningTag(temp.Value, _dateTimeProvider);
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<BookIds>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<BookIds>.Error(saveResult.Errors);
        }

        return Result<BookIds>.Success(new BookIds(id, fid));
    }

    public async Task<Result> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteBookByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.BookId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteBookCoverImageByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.BookId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        if (result.Value.AvailableCovers is null)
        {
            return Result.Succeeded;
        }

        var deleteResult = await _coverImageRepository
            .DeleteByIdAsync(result.Value.AvailableCovers.Id, cancellationToken);
        if (deleteResult.IsError)
        {
            return deleteResult;
        }

        result.Value.RemoveCovers(_dateTimeProvider);

        var updateResult = await _repository.UpdateAsync(result.Value, cancellationToken);
        if (updateResult.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateBookCoverImageByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.BookId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        result.Value.ChangeCovers(request.CoverImageMetaDataContainer, _dateTimeProvider);

        var updateResult = await _repository.UpdateAsync(result.Value, cancellationToken);
        if (updateResult.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateBookByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.BookId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        var userId = _currentUserService.UserId;

        if (result.Value.Title.Value != request.Title.Value)
        {
            result.Value.ChangeTitle(request.Title, _dateTimeProvider);
        }

        if (request.DescriptionText is not null)
        {
            result.Value.ChangeDescription(request.DescriptionText, _dateTimeProvider);
        }
        else
        {
            result.Value.RemoveDescription(_dateTimeProvider);
        }

        if (request.LiteratureForm is not null)
        {
            var dateTime = _dateTimeProvider.UtcNow;

            if (request.LiteratureForm.LiteratureFormId.HasValue && result.Value.LiteratureForm is not null)
            {
                if (request.LiteratureForm.LiteratureFormId.Value != result.Value.LiteratureForm.Id)
                {
                    var tempId = request.LiteratureForm.LiteratureFormId ?? LiteratureFormId.Create();
                    var temp = LiteratureForm.Create(tempId, dateTime, dateTime, userId,
                        request.LiteratureForm.ShortName, request.LiteratureForm.ScoreMultiplier);
                    if (temp.IsError)
                    {
                        return Result<BookIds>.Error(temp.Errors);
                    }

                    result.Value.ChangeLiteratureForm(temp.Value, _dateTimeProvider);
                }
            }
            else
            {
                var tempId = request.LiteratureForm.LiteratureFormId ?? LiteratureFormId.Create();
                var temp = LiteratureForm.Create(tempId, dateTime, dateTime, userId,
                    request.LiteratureForm.ShortName, request.LiteratureForm.ScoreMultiplier);
                if (temp.IsError)
                {
                    return Result<BookIds>.Error(temp.Errors);
                }

                result.Value.ChangeLiteratureForm(temp.Value, _dateTimeProvider);
            }
        }
        else
        {
            result.Value.RemoveLiteratureForm(_dateTimeProvider);
        }

        if (request.SeriesDto is not null)
        {
            var dateTime = _dateTimeProvider.UtcNow;

            if (request.SeriesDto.SeriesId.HasValue && result.Value.Series is not null)
            {
                if (request.SeriesDto.SeriesId.Value != result.Value.Series.Id)
                {
                    var tempId = request.SeriesDto.SeriesId ?? SeriesId.Create();
                    var temp = Domain.Entities.Series.Create(tempId, dateTime, dateTime, SeriesFriendlyId.Create(),
                        (UserVm?)userId, request.SeriesDto.Title);
                    if (temp.IsError)
                    {
                        return Result<BookIds>.Error(temp.Errors);
                    }

                    result.Value.ChangeSeries(temp.Value, _dateTimeProvider);
                    result.Value.ChangeNumberInSeries(request.SeriesDto.NumberInSeries, _dateTimeProvider);
                }
            }
            else
            {
                var tempId = request.SeriesDto.SeriesId ?? SeriesId.Create();
                var temp = Domain.Entities.Series.Create(tempId, dateTime, dateTime, SeriesFriendlyId.Create(),
                    (UserVm?)userId, request.SeriesDto.Title);
                if (temp.IsError)
                {
                    return Result<BookIds>.Error(temp.Errors);
                }

                result.Value.ChangeSeries(temp.Value, _dateTimeProvider);
                result.Value.ChangeNumberInSeries(request.SeriesDto.NumberInSeries, _dateTimeProvider);
            }
        }
        else
        {
            result.Value.RemoveSeries(_dateTimeProvider);
        }

        var authorIds = new List<AuthorId>(request.Authors.Count);
        foreach (var input in request.Authors)
        {
            var dateTime = _dateTimeProvider.UtcNow;

            var tempId = input.AuthorId ?? AuthorId.Create();
            authorIds.Add(tempId);
            var temp = Author.Create(tempId, dateTime, dateTime, AuthorFriendlyId.Create(),
                (UserVm?)userId, input.Name, null);
            if (temp.IsError)
            {
                return Result<BookIds>.Error(temp.Errors);
            }

            result.Value.AddAuthor(temp.Value, _dateTimeProvider);
        }

        foreach (var id in result.Value.Authors.Select(x => x.Id).ToList())
        {
            if (!authorIds.Contains(id))
            {
                result.Value.RemoveAuthorById(id, _dateTimeProvider);
            }
        }

        var genreIds = new List<GenreId>(request.Genres.Count);
        foreach (var input in request.Genres)
        {
            var dateTime = _dateTimeProvider.UtcNow;

            var tempId = input.GenreId ?? GenreId.Create();
            genreIds.Add(tempId);
            var temp = Genre.Create(tempId, dateTime, dateTime, userId,
                input.ShortName, input.IsFiction);
            if (temp.IsError)
            {
                return Result<BookIds>.Error(temp.Errors);
            }

            result.Value.AddGenre(temp.Value, _dateTimeProvider);
        }

        foreach (var id in result.Value.Genres.Select(x => x.Id).ToList())
        {
            if (!genreIds.Contains(id))
            {
                result.Value.RemoveGenreById(id, _dateTimeProvider);
            }
        }

        var tagIds = new List<TagId>(request.Tags.Count);
        foreach (var input in request.Tags)
        {
            var dateTime = _dateTimeProvider.UtcNow;

            var tempId = input.TagId ?? TagId.Create();
            tagIds.Add(tempId);
            var temp = Tag.Create(tempId, dateTime, dateTime, userId,
                input.Name);
            if (temp.IsError)
            {
                return Result<BookIds>.Error(temp.Errors);
            }

            result.Value.AddTag(temp.Value, _dateTimeProvider);
        }

        foreach (var id in result.Value.Tags.Select(x => x.Id).ToList())
        {
            if (!tagIds.Contains(id))
            {
                result.Value.RemoveTagById(id, _dateTimeProvider);
            }
        }

        var warningTagIds = new List<WarningTagId>(request.WarningTags.Count);
        foreach (var input in request.WarningTags)
        {
            var dateTime = _dateTimeProvider.UtcNow;

            var tempId = input.WarningTagId ?? WarningTagId.Create();
            warningTagIds.Add(tempId);
            var temp = WarningTag.Create(tempId, dateTime, dateTime, userId,
                input.Name);
            if (temp.IsError)
            {
                return Result<BookIds>.Error(temp.Errors);
            }

            result.Value.AddWarningTag(temp.Value, _dateTimeProvider);
        }

        foreach (var id in result.Value.WarningTags.Select(x => x.Id).ToList())
        {
            if (!warningTagIds.Contains(id))
            {
                result.Value.RemoveWarningTagById(id, _dateTimeProvider);
            }
        }

        var updateResult = await _repository.UpdateAsync(result.Value, cancellationToken);
        if (updateResult.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}