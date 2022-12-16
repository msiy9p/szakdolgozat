using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Posts.UpdatePost;

public sealed class
    UpdatePostCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdatePostCommand>
{
    public override void BuildPolicy(UpdatePostCommand instance)
    {
        UseRequirement(new SameCreatorRequirement(instance.Item.CreatorId));
        UseRequirement(new PostNotLockedRequirement(instance.Item.Id));
    }
}