using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Comments.DeleteComment;

public sealed class
    DeleteCommentCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteCommentCommand>
{
    private readonly ICurrentPostService _currentPostService;

    public DeleteCommentCommandAuthorisationPolicyBuilder(ICurrentPostService currentPostService)
    {
        _currentPostService = currentPostService;
    }

    public override void BuildPolicy(DeleteCommentCommand instance)
    {
        UseRequirement(new SameCreatorOrAboveRequirement(instance.Item.CreatorId));

        var postId = _currentPostService.CurrentPostId;
        if (!postId.HasValue)
        {
            // Throw exception?
            UseRequirement(FailAuthorisationRequirement.Instance);
        }
        else
        {
            UseRequirement(new PostNotLockedRequirement(postId.Value));
        }
    }
}