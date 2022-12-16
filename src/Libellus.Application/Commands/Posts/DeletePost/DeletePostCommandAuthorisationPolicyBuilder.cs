using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Posts.DeletePost;

public sealed class
    DeletePostCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeletePostCommand>
{
    public override void BuildPolicy(DeletePostCommand instance)
    {
        UseRequirement(new SameCreatorOrAboveRequirement(instance.Item.CreatorId));
    }
}