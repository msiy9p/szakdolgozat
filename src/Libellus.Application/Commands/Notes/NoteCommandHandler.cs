using Libellus.Application.Commands.Notes.CreateNote;
using Libellus.Application.Commands.Notes.DeleteNote;
using Libellus.Application.Commands.Notes.DeleteNoteById;
using Libellus.Application.Commands.Notes.UpdateNote;
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
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Commands.Notes;

public sealed class NoteCommandHandler :
    ICommandHandler<CreateNoteCommand, NoteId>,
    ICommandHandler<DeleteNoteCommand>,
    ICommandHandler<DeleteNoteByIdCommand>,
    ICommandHandler<UpdateNoteCommand>
{
    private readonly INoteRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IReadingReadOnlyRepository _readingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHtmlSanitizer _htmlSanitizer;

    public NoteCommandHandler(INoteRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IReadingReadOnlyRepository readingRepository, IUnitOfWork unitOfWork,
        IHtmlSanitizer htmlSanitizer)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _readingRepository = readingRepository;
        _unitOfWork = unitOfWork;
        _htmlSanitizer = htmlSanitizer;
    }

    public async Task<Result<NoteId>> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var readingResult = await _readingRepository.ExistsAsync(request.ReadingId, cancellationToken);
        if (readingResult.IsError)
        {
            return Result<NoteId>.Error(readingResult.Errors);
        }

        if (!readingResult.Value)
        {
            return DomainErrors.ReadingErrors.ReadingNotFound.ToErrorResult<NoteId>();
        }

        var sanitizedComment = _htmlSanitizer.Sanitize(request.CommentText.Value);
        var newCommentText = CommentText.Create(sanitizedComment);
        if (newCommentText.IsError)
        {
            return Result<NoteId>.Error(newCommentText.Errors);
        }

        var dateTime = _dateTimeProvider.UtcNow;
        var id = NoteId.Create();
        var item = Note.Create(id, dateTime, dateTime, (UserPicturedVm?)_currentUserService.UserId,
            newCommentText.Value);

        if (item.IsError)
        {
            return Result<NoteId>.Error(item.Errors);
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<NoteId>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<NoteId>.Error(saveResult.Errors);
        }

        return Result<NoteId>.Success(id);
    }

    public async Task<Result> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteNoteByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.NoteId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var sanitizedComment = _htmlSanitizer.Sanitize(request.Item.Text.Value);
        var newCommentText = CommentText.Create(sanitizedComment);
        if (newCommentText.IsError)
        {
            return Result<CommentIds>.Error(newCommentText.Errors);
        }

        request.Item.ChangeText(newCommentText.Value, _dateTimeProvider);

        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}