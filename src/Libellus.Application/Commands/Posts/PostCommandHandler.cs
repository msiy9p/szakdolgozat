using Libellus.Application.Commands.Posts.CreatePost;
using Libellus.Application.Commands.Posts.DeletePost;
using Libellus.Application.Commands.Posts.DeletePostById;
using Libellus.Application.Commands.Posts.LockPostById;
using Libellus.Application.Commands.Posts.UnlockPostById;
using Libellus.Application.Commands.Posts.UpdatePost;
using Libellus.Application.Commands.Posts.UpdatePostById;
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

namespace Libellus.Application.Commands.Posts;

public sealed class PostCommandHandler :
    ICommandHandler<CreatePostCommand, PostIds>,
    ICommandHandler<DeletePostCommand>,
    ICommandHandler<DeletePostByIdCommand>,
    ICommandHandler<UpdatePostCommand>,
    ICommandHandler<UpdatePostByIdCommand>,
    ICommandHandler<LockPostByIdCommand>,
    ICommandHandler<UnlockPostByIdCommand>
{
    private readonly IPostRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILabelRepository _labelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHtmlSanitizer _htmlSanitizer;

    public PostCommandHandler(IPostRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, ILabelRepository labelRepository, IUnitOfWork unitOfWork,
        IHtmlSanitizer htmlSanitizer)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _labelRepository = labelRepository;
        _unitOfWork = unitOfWork;
        _htmlSanitizer = htmlSanitizer;
    }

    public async Task<Result<PostIds>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        Label? label = null;
        if (request.Label is not null)
        {
            var labelResult = await _labelRepository.FindByNameAsync(request.Label, cancellationToken);
            if (labelResult.IsError && !labelResult.Errors.Contains(DomainErrors.LabelErrors.LabelNotFound))
            {
                return await Task.FromResult(Result<PostIds>.Error(labelResult.Errors));
            }

            if (labelResult.IsError && labelResult.Errors.Contains(DomainErrors.LabelErrors.LabelNotFound))
            {
                //var time = _dateTimeProvider.UtcNow;
                //var tempLabel = Label.Create(LabelId.Create(), time, time, request.Label!);

                //if (tempLabel.IsSuccess)
                //{
                //    var tempLabelResult = await _labelRepository.AddAsync(tempLabel.Value!, cancellationToken);
                //    if (tempLabelResult.IsError)
                //    {
                //        return Result<PostIds>.Error(tempLabelResult.Errors);
                //    }

                //    label = tempLabel.Value;
                //}
            }
            else
            {
                label = labelResult.Value;
            }
        }

        var sanitizedComment = _htmlSanitizer.Sanitize(request.CommentText.Value);
        var newCommentText = CommentText.Create(sanitizedComment);
        if (newCommentText.IsError)
        {
            return Result<PostIds>.Error(newCommentText.Errors);
        }

        var dateTime = _dateTimeProvider.UtcNow;
        var id = PostId.Create();
        var fid = PostFriendlyId.Create();
        var item = Post.Create(id, dateTime, dateTime, fid, (UserPicturedVm?)_currentUserService.UserId, label,
            request.Title,
            newCommentText.Value, request.IsMemberOnly, request.IsSpoiler);

        if (item.IsError)
        {
            return await Task.FromResult(Result<PostIds>.Error(item.Errors));
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<PostIds>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<PostIds>.Error(saveResult.Errors);
        }

        return Result<PostIds>.Success(new PostIds(id, fid));
    }

    public async Task<Result> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeletePostByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.PostId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var sanitizedComment = _htmlSanitizer.Sanitize(request.Item.Text.Value);
        var newCommentText = CommentText.Create(sanitizedComment);
        if (newCommentText.IsError)
        {
            return Result<PostIds>.Error(newCommentText.Errors);
        }

        request.Item.ChangeText(newCommentText.Value, _dateTimeProvider);

        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdatePostByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.PostId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        var post = result.Value;

        var sanitizedComment = _htmlSanitizer.Sanitize(request.CommentText.Value);
        var newCommentText = CommentText.Create(sanitizedComment);
        if (newCommentText.IsError)
        {
            return Result<CommentIds>.Error(newCommentText.Errors);
        }

        post!.ChangeText(newCommentText.Value, _dateTimeProvider);

        post!.ChangeMemberOnly(request.IsMemberOnly, _dateTimeProvider);
        post!.ChangeSpoiler(request.IsSpoiler, _dateTimeProvider);

        if ((post!.Label is null && request.Label is not null) || (post!.Label is not null &&
                                                                   request.Label is not null &&
                                                                   post!.Label.Name != request.Label))
        {
            var labelResult = await _labelRepository.FindByNameAsync(request.Label, cancellationToken);
            if (labelResult.IsError && !labelResult.Errors.Contains(DomainErrors.LabelErrors.LabelNotFound))
            {
                return labelResult;
            }

            if (labelResult.IsError && labelResult.Errors.Contains(DomainErrors.LabelErrors.LabelNotFound))
            {
                //var time = _dateTimeProvider.UtcNow;
                //var tempLabel = Label.Create(LabelId.Create(), time, time, request.Label!);

                //if (tempLabel.IsSuccess)
                //{
                //    var tempLabelResult = await _labelRepository.AddAsync(tempLabel.Value!, cancellationToken);
                //    if (tempLabelResult.IsError)
                //    {
                //        return Result<PostIds>.Error(tempLabelResult.Errors);
                //    }

                //    post!.ChangeLabel(tempLabel.Value!, _dateTimeProvider);
                //}
            }
            else
            {
                post!.ChangeLabel(labelResult.Value!, _dateTimeProvider);
            }
        }
        else if (post!.Label is not null && request.Label is null)
        {
            post!.RemoveLabel(_dateTimeProvider);
        }

        var updateResult = await _repository.UpdateAsync(post!, cancellationToken);
        if (updateResult.IsError)
        {
            return updateResult;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(LockPostByIdCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return DomainErrors.UserErrors.UserNotFound.ToErrorResult();
        }

        var result = await _repository.GetByIdAsync(request.PostId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        result.Value.Lock(userId.Value, request.CommentText, _dateTimeProvider);

        var updateResult = await _repository.UpdateAsync(result.Value!, cancellationToken);
        if (updateResult.IsError)
        {
            return updateResult;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UnlockPostByIdCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return DomainErrors.UserErrors.UserNotFound.ToErrorResult();
        }

        var result = await _repository.GetByIdAsync(request.PostId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        result.Value.Unlock(userId.Value, _dateTimeProvider);

        var updateResult = await _repository.UpdateAsync(result.Value!, cancellationToken);
        if (updateResult.IsError)
        {
            return updateResult;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}