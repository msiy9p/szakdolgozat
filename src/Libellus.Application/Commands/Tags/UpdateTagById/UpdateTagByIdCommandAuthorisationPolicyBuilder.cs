using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Tags.UpdateTagById;

public sealed class
    UpdateTagByIdCommandAuthorisationPolicyBuilder :
        BaseAuthorisationPolicyBuilder<UpdateTagByIdCommand>
{
    private readonly ITagReadOnlyRepository _repository;

    public UpdateTagByIdCommandAuthorisationPolicyBuilder(ITagReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdateTagByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.TagId);
        if (result.IsError)
        {
            UseRequirement(new SameCreatorOrAboveRequirement(null));
        }
        else
        {
            UseRequirement(new SameCreatorOrAboveRequirement(result.Value));
        }
    }
}