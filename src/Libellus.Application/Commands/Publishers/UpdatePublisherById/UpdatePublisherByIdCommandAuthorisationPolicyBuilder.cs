using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Publishers.UpdatePublisherById;

public sealed class
    UpdatePublisherByIdCommandAuthorisationPolicyBuilder :
        BaseAuthorisationPolicyBuilder<UpdatePublisherByIdCommand>
{
    private readonly IPublisherReadOnlyRepository _repository;

    public UpdatePublisherByIdCommandAuthorisationPolicyBuilder(IPublisherReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdatePublisherByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.PublisherId);
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