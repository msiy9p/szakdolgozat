using Libellus.Application.Commands.Readings.CreateReading;
using Libellus.Application.Commands.Readings.DeleteReading;
using Libellus.Application.Commands.Readings.DeleteReadingById;
using Libellus.Application.Commands.Readings.UpdateReading;
using Libellus.Application.Commands.Readings.UpdateReadingById;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Commands.Readings;

public sealed class ReadingCommandHandler :
    ICommandHandler<CreateReadingCommand, ReadingIds>,
    ICommandHandler<DeleteReadingCommand>,
    ICommandHandler<DeleteReadingByIdCommand>,
    ICommandHandler<UpdateReadingCommand>,
    ICommandHandler<UpdateReadingByIdCommand>
{
    private readonly IReadingRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IBookEditionReadOnlyRepository _bookEditionRepository;
    private readonly IBookReadOnlyRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReadingCommandHandler(IReadingRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IBookEditionReadOnlyRepository bookEditionRepository,
        IUnitOfWork unitOfWork, IBookReadOnlyRepository bookRepository)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _bookEditionRepository = bookEditionRepository;
        _unitOfWork = unitOfWork;
        _bookRepository = bookRepository;
    }

    public async Task<Result<ReadingIds>> Handle(CreateReadingCommand request, CancellationToken cancellationToken)
    {
        var bookEditionResult = await _bookEditionRepository.GetByIdAsync(request.BookEditionId, cancellationToken);
        if (bookEditionResult.IsError)
        {
            return Result<ReadingIds>.Error(bookEditionResult.Errors);
        }

        var bookResult = await _bookRepository.GetByIdAsync(bookEditionResult.Value.BookId, cancellationToken);
        if (bookEditionResult.IsError)
        {
            return Result<ReadingIds>.Error(bookEditionResult.Errors);
        }

        var dateTime = _dateTimeProvider.UtcNow;
        var id = ReadingId.Create();
        var fid = ReadingFriendlyId.Create();
        var readingResult = Reading.Create(id, dateTime, dateTime, fid,
            ((UserPicturedVm)_currentUserService.UserId)!, new BookEditionCompactVm(request.BookEditionId,
                BookEditionFriendlyId.Create(),
                new Title(request.BookEditionId.ToString()), Array.Empty<AuthorVm>()),
            bookEditionResult.Value.PageCount,
            bookResult.Value.LiteratureForm?.ScoreMultiplier, null,
            request.IsDnf, request.IsReread, _dateTimeProvider.ToZonedDateTimeUtc(request.StartedOnUtc),
            _dateTimeProvider.ToZonedDateTimeUtc(request.FinishedOnUtc));
        if (readingResult.IsError)
        {
            return Result<ReadingIds>.Error(readingResult.Errors);
        }

        var result = await _repository.AddIfNotExistsAsync(readingResult.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<ReadingIds>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<ReadingIds>.Error(saveResult.Errors);
        }

        return new ReadingIds(id, fid).ToResult();
    }

    public async Task<Result> Handle(DeleteReadingCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteReadingByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.ReadingId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateReadingCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateReadingByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.ReadingId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        if (request.CommentText is not null)
        {
            if (result.Value.Note is not null)
            {
                result.Value.Note.ChangeText(request.CommentText, _dateTimeProvider);
            }
            else
            {
                var datetime = _dateTimeProvider.UtcNow;

                var note = Note.Create(NoteId.Create(), datetime, datetime,
                    ((UserPicturedVm)_currentUserService.UserId)!, request.CommentText);
                if (note.IsError)
                {
                    return note;
                }

                result.Value.AddNote(note.Value, _dateTimeProvider);
            }
        }
        else
        {
            result.Value.RemoveNote(_dateTimeProvider);
        }

        if (request.StartedOnUtc.HasValue)
        {
            var temp = _dateTimeProvider.ToZonedDateTimeUtc(request.StartedOnUtc.Value);

            if (result.Value.StartedOnUtc.HasValue)
            {
                if (result.Value.StartedOnUtc != temp.Value)
                {
                    result.Value.StartReading(temp.Value, _dateTimeProvider);
                }
            }
            else
            {
                result.Value.StartReading(temp.Value, _dateTimeProvider);
            }
        }
        else
        {
            result.Value.RemoveStartedOnUtc(_dateTimeProvider);
        }

        if (request.FinishedOnUtc.HasValue)
        {
            var temp = _dateTimeProvider.ToZonedDateTimeUtc(request.FinishedOnUtc.Value);

            if (result.Value.FinishedOnUtc.HasValue)
            {
                if (result.Value.FinishedOnUtc != temp.Value)
                {
                    result.Value.FinishReading(temp.Value, _dateTimeProvider);
                }
            }
            else
            {
                result.Value.FinishReading(temp.Value, _dateTimeProvider);
            }
        }
        else
        {
            result.Value.RemoveFinishedOnUtc(_dateTimeProvider);
        }

        result.Value.ChangeDnf(request.IsDnf, _dateTimeProvider);
        result.Value.ChangeReread(request.IsReread, _dateTimeProvider);

        var updateResult = await _repository.UpdateAsync(result.Value, cancellationToken);
        if (updateResult.IsError)
        {
            return updateResult;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}