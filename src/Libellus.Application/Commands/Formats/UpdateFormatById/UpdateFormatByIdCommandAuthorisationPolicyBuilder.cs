using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Formats.UpdateFormatById;

public sealed class
    UpdateFormatByIdCommandAuthorisationPolicyBuilder :
        BaseAuthorisationPolicyBuilder<UpdateFormatByIdCommand>
{
    private readonly IFormatReadOnlyRepository _repository;

    public UpdateFormatByIdCommandAuthorisationPolicyBuilder(IFormatReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdateFormatByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.FormatId);
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