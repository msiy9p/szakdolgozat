using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.LiteratureForms.UpdateLiteratureFormById;

public sealed class
    UpdateLiteratureFormByIdCommandAuthorisationPolicyBuilder :
        BaseAuthorisationPolicyBuilder<UpdateLiteratureFormByIdCommand>
{
    private readonly ILiteratureFormReadOnlyRepository _repository;

    public UpdateLiteratureFormByIdCommandAuthorisationPolicyBuilder(ILiteratureFormReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdateLiteratureFormByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.LiteratureFormId);
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