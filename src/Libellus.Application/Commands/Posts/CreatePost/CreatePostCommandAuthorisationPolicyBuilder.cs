using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Posts.CreatePost;

public sealed class
    CreatePostCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<CreatePostCommand>
{
    public override void BuildPolicy(CreatePostCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);
    }
}