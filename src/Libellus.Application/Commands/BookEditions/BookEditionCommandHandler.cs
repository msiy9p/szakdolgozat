using System.Runtime.InteropServices.ComTypes;
using Libellus.Application.Commands.BookEditions.CreateBookEdition;
using Libellus.Application.Commands.BookEditions.DeleteBookEdition;
using Libellus.Application.Commands.BookEditions.DeleteBookEditionById;
using Libellus.Application.Commands.BookEditions.DeleteBookEditionCoverImageById;
using Libellus.Application.Commands.BookEditions.UpdateBookEdition;
using Libellus.Application.Commands.BookEditions.UpdateBookEditionById;
using Libellus.Application.Commands.BookEditions.UpdateBookEditionCoverImageById;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Commands.BookEditions;

public sealed class BookEditionCommandHandler :
    ICommandHandler<CreateBookEditionCommand, BookEditionIds>,
    ICommandHandler<DeleteBookEditionCommand>,
    ICommandHandler<DeleteBookEditionByIdCommand>,
    ICommandHandler<DeleteBookEditionCoverImageByIdCommand>,
    ICommandHandler<UpdateBookEditionCommand>,
    ICommandHandler<UpdateBookEditionByIdCommand>,
    ICommandHandler<UpdateBookEditionCoverImageByIdCommand>
{
    private readonly IBookEditionRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IBookReadOnlyRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICoverImageRepository _coverImageRepository;

    public BookEditionCommandHandler(IBookEditionRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork, IBookReadOnlyRepository bookRepository,
        ICoverImageRepository coverImageRepository)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _bookRepository = bookRepository;
        _coverImageRepository = coverImageRepository;
    }

    public async Task<Result<BookEditionIds>> Handle(CreateBookEditionCommand request,
        CancellationToken cancellationToken)
    {
        var bookResult = await _bookRepository.ExistsAsync(request.BookId, cancellationToken);
        if (bookResult.IsError)
        {
            return Result<BookEditionIds>.Error(bookResult.Errors);
        }

        if (!bookResult.Value)
        {
            return DomainErrors.BookErrors.BookNotFound.ToErrorResult<BookEditionIds>();
        }

        var userId = _currentUserService.UserId;
        var dateTime = _dateTimeProvider.UtcNow;
        var id = BookEditionId.Create();
        var fid = BookEditionFriendlyId.Create();
        var item = BookEdition.Create(id, dateTime, dateTime, fid,
            (UserVm?)userId, new BookCompactVm(request.BookId, BookFriendlyId.Create(),
                new Title(request.BookId.ToString()), Array.Empty<AuthorVm>()),
            request.Title, request.DescriptionText, request.IsTranslation,
            request.CoverImageMetaDataContainer);

        if (item.IsError)
        {
            return await Task.FromResult(Result<BookEditionIds>.Error(item.Errors));
        }

        if (request.Format is not null)
        {
            var tempId = request.Format.FormatId ?? FormatId.Create();

            var temp = Format.Create(tempId, dateTime, dateTime, userId,
                request.Format.Name, request.Format.IsDigital);

            if (temp.IsError)
            {
                return Result<BookEditionIds>.Error(temp.Errors);
            }

            item.Value.ChangeFormat(temp.Value, _dateTimeProvider);
        }

        if (request.Language is not null)
        {
            var tempId = request.Language.LanguageId ?? LanguageId.Create();

            var temp = Language.Create(tempId, dateTime, dateTime, userId,
                request.Language.Name);

            if (temp.IsError)
            {
                return Result<BookEditionIds>.Error(temp.Errors);
            }

            item.Value.ChangeLanguage(temp.Value, _dateTimeProvider);
        }

        if (request.Publisher is not null)
        {
            var tempId = request.Publisher.PublisherId ?? PublisherId.Create();

            var temp = Publisher.Create(tempId, dateTime, dateTime, userId,
                request.Publisher.Name);

            if (temp.IsError)
            {
                return Result<BookEditionIds>.Error(temp.Errors);
            }

            item.Value.ChangePublisher(temp.Value, _dateTimeProvider);
        }

        if (request.PublishedOn.HasValue)
        {
            item.Value.ChangePublishedOn(request.PublishedOn.Value, _dateTimeProvider);
        }

        if (request.WordCount.HasValue)
        {
            item.Value.ChangeWordCount(request.WordCount.Value, _dateTimeProvider);
        }

        if (request.PageCount.HasValue)
        {
            item.Value.ChangePageCount(request.PageCount.Value, _dateTimeProvider);
        }

        if (request.Isbn.HasValue)
        {
            item.Value.ChangeIsbn(request.Isbn.Value, _dateTimeProvider);
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<BookEditionIds>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<BookEditionIds>.Error(saveResult.Errors);
        }

        return new BookEditionIds(id, fid).ToResult();
    }

    public async Task<Result> Handle(DeleteBookEditionCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteBookEditionByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.BookEditionId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateBookEditionCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteBookEditionCoverImageByIdCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.BookEditionId, cancellationToken);
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

    public async Task<Result> Handle(UpdateBookEditionCoverImageByIdCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.BookEditionId, cancellationToken);
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

    public async Task<Result> Handle(UpdateBookEditionByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.BookEditionId, cancellationToken);
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

        if (request.Format is not null)
        {
            var dateTime = _dateTimeProvider.UtcNow;

            if (request.Format.FormatId.HasValue && result.Value.Format is not null)
            {
                if (request.Format.FormatId.Value != result.Value.Format.Id)
                {
                    var tempId = request.Format.FormatId ?? FormatId.Create();
                    var temp = Format.Create(tempId, dateTime, dateTime, userId,
                        request.Format.Name, request.Format.IsDigital);

                    if (temp.IsError)
                    {
                        return Result<BookEditionIds>.Error(temp.Errors);
                    }

                    result.Value.ChangeFormat(temp.Value, _dateTimeProvider);
                }
            }
            else
            {
                var tempId = request.Format.FormatId ?? FormatId.Create();
                var temp = Format.Create(tempId, dateTime, dateTime, userId,
                    request.Format.Name, request.Format.IsDigital);

                if (temp.IsError)
                {
                    return Result<BookEditionIds>.Error(temp.Errors);
                }

                result.Value.ChangeFormat(temp.Value, _dateTimeProvider);
            }
        }
        else
        {
            result.Value.RemoveFormat(_dateTimeProvider);
        }

        if (request.Language is not null)
        {
            var dateTime = _dateTimeProvider.UtcNow;

            if (request.Language.LanguageId.HasValue && result.Value.Language is not null)
            {
                if (request.Language.LanguageId.Value != result.Value.Language.Id)
                {
                    var tempId = request.Language.LanguageId ?? LanguageId.Create();
                    var temp = Language.Create(tempId, dateTime, dateTime, userId,
                        request.Language.Name);

                    if (temp.IsError)
                    {
                        return Result<BookEditionIds>.Error(temp.Errors);
                    }

                    result.Value.ChangeLanguage(temp.Value, _dateTimeProvider);
                }
            }
            else
            {
                var tempId = request.Language.LanguageId ?? LanguageId.Create();
                var temp = Language.Create(tempId, dateTime, dateTime, userId,
                    request.Language.Name);

                if (temp.IsError)
                {
                    return Result<BookEditionIds>.Error(temp.Errors);
                }

                result.Value.ChangeLanguage(temp.Value, _dateTimeProvider);
            }
        }
        else
        {
            result.Value.RemoveLanguage(_dateTimeProvider);
        }

        if (request.Publisher is not null)
        {
            var dateTime = _dateTimeProvider.UtcNow;

            if (request.Publisher.PublisherId.HasValue && result.Value.Publisher is not null)
            {
                if (request.Publisher.PublisherId.Value != result.Value.Publisher.Id)
                {
                    var tempId = request.Publisher.PublisherId ?? PublisherId.Create();
                    var temp = Publisher.Create(tempId, dateTime, dateTime, userId,
                        request.Publisher.Name);

                    if (temp.IsError)
                    {
                        return Result<BookEditionIds>.Error(temp.Errors);
                    }

                    result.Value.ChangePublisher(temp.Value, _dateTimeProvider);
                }
            }
            else
            {
                var tempId = request.Publisher.PublisherId ?? PublisherId.Create();
                var temp = Publisher.Create(tempId, dateTime, dateTime, userId,
                    request.Publisher.Name);

                if (temp.IsError)
                {
                    return Result<BookEditionIds>.Error(temp.Errors);
                }

                result.Value.ChangePublisher(temp.Value, _dateTimeProvider);
            }
        }
        else
        {
            result.Value.RemovePublisher(_dateTimeProvider);
        }

        if (request.PublishedOn.HasValue)
        {
            if (result.Value.PublishedOn.HasValue)
            {
                if (request.PublishedOn.Value != result.Value.PublishedOn.Value)
                {
                    result.Value.ChangePublishedOn(request.PublishedOn.Value, _dateTimeProvider);
                }
            }
            else
            {
                result.Value.ChangePublishedOn(request.PublishedOn.Value, _dateTimeProvider);
            }
        }
        else
        {
            result.Value.RemovePublishedOn(_dateTimeProvider);
        }

        if (request.WordCount.HasValue)
        {
            if (result.Value.WordCount.HasValue)
            {
                if (request.WordCount.Value != result.Value.WordCount.Value)
                {
                    result.Value.ChangeWordCount(request.WordCount.Value, _dateTimeProvider);
                }
            }
            else
            {
                result.Value.ChangeWordCount(request.WordCount.Value, _dateTimeProvider);
            }
        }
        else
        {
            result.Value.RemoveWordCount(_dateTimeProvider);
        }

        if (request.PageCount.HasValue)
        {
            if (result.Value.PageCount.HasValue)
            {
                if (request.PageCount.Value != result.Value.PageCount.Value)
                {
                    result.Value.ChangePageCount(request.PageCount.Value, _dateTimeProvider);
                }
            }
            else
            {
                result.Value.ChangePageCount(request.PageCount.Value, _dateTimeProvider);
            }
        }
        else
        {
            result.Value.RemovePageCount(_dateTimeProvider);
        }

        if (request.Isbn.HasValue)
        {
            if (result.Value.Isbn.HasValue)
            {
                if (request.Isbn.Value != result.Value.Isbn.Value)
                {
                    result.Value.ChangeIsbn(request.Isbn.Value, _dateTimeProvider);
                }
            }
            else
            {
                result.Value.ChangeIsbn(request.Isbn.Value, _dateTimeProvider);
            }
        }
        else
        {
            result.Value.RemoveIsbn(_dateTimeProvider);
        }

        result.Value.ChangeTranslation(request.IsTranslation, _dateTimeProvider);

        var updateResult = await _repository.UpdateAsync(result.Value, cancellationToken);
        if (updateResult.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}