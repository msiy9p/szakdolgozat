using Libellus.Application.Commands.Comments.CreateComment;
using Libellus.Application.Commands.Comments.DeleteComment;
using Libellus.Application.Commands.Comments.DeleteCommentById;
using Libellus.Application.Commands.Comments.UpdateComment;
using Libellus.Application.Commands.Comments.UpdateCommentById;
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

namespace Libellus.Application.Commands.Comments;

public sealed class CommentCommandHandler :
    ICommandHandler<CreateCommentCommand, CommentIds>,
    ICommandHandler<DeleteCommentCommand>,
    ICommandHandler<DeleteCommentByIdCommand>,
    ICommandHandler<UpdateCommentCommand>,
    ICommandHandler<UpdateCommentByIdCommand>
{
    private readonly ICommentRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPostReadOnlyRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHtmlSanitizer _htmlSanitizer;

    public CommentCommandHandler(ICommentRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IPostReadOnlyRepository postRepository, IUnitOfWork unitOfWork,
        IHtmlSanitizer htmlSanitizer)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
        _htmlSanitizer = htmlSanitizer;
    }

    public async Task<Result<CommentIds>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (!userId.HasValue)
        {
            return DomainErrors.UserErrors.UserNotFound.ToErrorResult<CommentIds>();
        }

        var postResult = await _postRepository.ExistsAsync(request.PostId, cancellationToken);
        if (postResult.IsError)
        {
            return Result<CommentIds>.Error(postResult.Errors);
        }

        if (!postResult.Value)
        {
            return DomainErrors.PostErrors.PostNotFound.ToErrorResult<CommentIds>();
        }

        var sanitizedComment = _htmlSanitizer.Sanitize(request.CommentText.Value);
        var newCommentText = CommentText.Create(sanitizedComment);
        if (newCommentText.IsError)
        {
            return Result<CommentIds>.Error(newCommentText.Errors);
        }

        var dateTime = _dateTimeProvider.UtcNow;
        var id = CommentId.Create();
        var fid = CommentFriendlyId.Create();
        var item = Comment.Create(id, dateTime, dateTime, fid, (UserPicturedVm)userId.Value!,
            newCommentText.Value, request.RepliedToCommentId);

        if (item.IsError)
        {
            return await Task.FromResult(Result<CommentIds>.Error(item.Errors));
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<CommentIds>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<CommentIds>.Error(saveResult.Errors);
        }

        return Result<CommentIds>.Success(new CommentIds(id, fid));
    }

    public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteCommentByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.CommentId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
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

    public async Task<Result> Handle(UpdateCommentByIdCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (!userId.HasValue)
        {
            return DomainErrors.UserErrors.UserNotFound.ToErrorResult<CommentIds>();
        }

        var commentResult = await _repository.GetByIdAsync(request.CommentId, cancellationToken);
        if (commentResult.IsError)
        {
            return Result<CommentIds>.Error(commentResult.Errors);
        }

        var sanitizedComment = _htmlSanitizer.Sanitize(request.CommentText.Value);
        var newCommentText = CommentText.Create(sanitizedComment);
        if (newCommentText.IsError)
        {
            return Result<CommentIds>.Error(newCommentText.Errors);
        }

        commentResult.Value!.ChangeText(newCommentText.Value, _dateTimeProvider);

        var result = await _repository.UpdateAsync(commentResult.Value!, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}