using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.WarningTags.UpdateWarningTagById;

public sealed class
    UpdateWarningTagByIdCommandAuthorisationPolicyBuilder :
        BaseAuthorisationPolicyBuilder<UpdateWarningTagByIdCommand>
{
    private readonly IWarningTagReadOnlyRepository _repository;

    public UpdateWarningTagByIdCommandAuthorisationPolicyBuilder(IWarningTagReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdateWarningTagByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.WarningTagId);
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