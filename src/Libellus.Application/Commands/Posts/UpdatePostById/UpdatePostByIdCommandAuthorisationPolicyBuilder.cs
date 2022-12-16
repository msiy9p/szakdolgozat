using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Posts.UpdatePostById;

public sealed class
    UpdatePostByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdatePostByIdCommand>
{
    private readonly IPostReadOnlyRepository _repository;

    public UpdatePostByIdCommandAuthorisationPolicyBuilder(IPostReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdatePostByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.PostId);
        if (result.IsError)
        {
            UseRequirement(new SameCreatorRequirement(null));
        }
        else
        {
            UseRequirement(new SameCreatorRequirement(result.Value));
        }

        UseRequirement(new PostNotLockedRequirement(instance.PostId));
    }
}