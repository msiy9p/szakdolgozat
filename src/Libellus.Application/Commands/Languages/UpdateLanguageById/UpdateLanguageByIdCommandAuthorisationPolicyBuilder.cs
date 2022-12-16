using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Languages.UpdateLanguageById;

public sealed class
    UpdateLanguageByIdCommandAuthorisationPolicyBuilder :
        BaseAuthorisationPolicyBuilder<UpdateLanguageByIdCommand>
{
    private readonly ILanguageReadOnlyRepository _repository;

    public UpdateLanguageByIdCommandAuthorisationPolicyBuilder(ILanguageReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdateLanguageByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.LanguageId);
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