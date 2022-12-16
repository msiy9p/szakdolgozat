#pragma warning disable CS8618
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Queries.Comments.GetCommentById;
using Libellus.Application.Queries.Posts.GetPostById;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Errors;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using LibellusWeb.Common;
using LibellusWeb.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibellusWeb.Pages.Group.Post.Comment;

public class CommentModel : LoggedPageModel<CommentModel>
{
    private readonly ISender _sender;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;
    private readonly ClaimsPrincipalExtractor _claimsPrincipalExtractor;

    public CommentModel(ILogger<CommentModel> logger, ISender sender,
        IFriendlyIdLookupRepository friendlyIdLookupRepository,
        ClaimsPrincipalExtractor claimsPrincipalExtractor) : base(logger)
    {
        _sender = sender;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
        _claimsPrincipalExtractor = claimsPrincipalExtractor;
    }

    public Libellus.Domain.Entities.Post Post { get; set; }

    public Libellus.Domain.Entities.Comment Comment { get; set; }

    public bool IsCreator { get; set; } = false;
    public UserId UserId { get; set; }

    public string GroupId { get; set; }
    public string PostId { get; set; }
    public string CommentId { get; set; }
    public string LinkBase { get; set; }

    private async Task<Result<PostId>> PostExistsAsync(string postFriendlyId,
        CancellationToken cancellationToken = default)
    {
        var postFriendlyIdId = PostFriendlyId.Convert(postFriendlyId);
        if (!postFriendlyIdId.HasValue)
        {
            return DomainErrors.PostErrors.PostNotFound.ToErrorResult<PostId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(postFriendlyIdId.Value, cancellationToken);
    }

    private async Task<Result<CommentId>> CommentExistsAsync(string friendlyId,
        CancellationToken cancellationToken = default)
    {
        var commentFriendlyId = CommentFriendlyId.Convert(friendlyId);
        if (!commentFriendlyId.HasValue)
        {
            return DomainErrors.CommentErrors.CommentNotFound.ToErrorResult<CommentId>();
        }

        return await _friendlyIdLookupRepository.LookupAsync(commentFriendlyId.Value, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(string gid, string pid, string cid)
    {
        GroupId = gid;
        PostId = pid;
        CommentId = cid;

        var result = _claimsPrincipalExtractor.GetUserId(User);
        if (result.IsError)
        {
            return NotFound("Unable to load user.");
        }

        UserId = result.Value;

        var postId = await PostExistsAsync(pid);
        if (postId.IsError)
        {
            return NotFound();
        }

        var commentId = await CommentExistsAsync(cid);
        if (commentId.IsError)
        {
            return NotFound();
        }

        var postQuery = new GetPostByIdQuery(postId.Value);
        var postQueryResult = await _sender.Send(postQuery);
        if (postQueryResult.IsError)
        {
            return NotFound();
        }

        var commentQuery = new GetCommentByIdQuery(commentId.Value);
        var commentQueryResult = await _sender.Send(commentQuery);
        if (commentQueryResult.IsError)
        {
            return NotFound();
        }

        Post = postQueryResult.Value;
        Comment = commentQueryResult.Value;

        if (Post.CreatorId is not null && Post.CreatorId.Value == result.Value)
        {
            IsCreator = true;
        }

        LinkBase = CreateProfilePictureUrlBase();

        return Page();
    }
}