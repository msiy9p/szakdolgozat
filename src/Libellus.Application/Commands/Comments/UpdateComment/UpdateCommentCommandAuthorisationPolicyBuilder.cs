using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Comments.UpdateComment;

public sealed class
    UpdateCommentCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdateCommentCommand>
{
    private readonly ICurrentPostService _currentPostService;

    public UpdateCommentCommandAuthorisationPolicyBuilder(ICurrentPostService currentPostService)
    {
        _currentPostService = currentPostService;
    }

    public override void BuildPolicy(UpdateCommentCommand instance)
    {
        UseRequirement(new SameCreatorRequirement(instance.Item.CreatorId));

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