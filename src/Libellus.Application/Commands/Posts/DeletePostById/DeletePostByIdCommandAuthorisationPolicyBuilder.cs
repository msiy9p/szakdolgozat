using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Posts.DeletePostById;

public sealed class
    DeletePostByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeletePostByIdCommand>
{
    private readonly IPostReadOnlyRepository _repository;

    public DeletePostByIdCommandAuthorisationPolicyBuilder(IPostReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(DeletePostByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.PostId);
        if (result.IsError)
        {
            UseRequirement(new SameCreatorOrAboveRequirement(result.Value));
        }

        UseRequirement(new SameCreatorOrAboveRequirement(result.Value));
    }
}