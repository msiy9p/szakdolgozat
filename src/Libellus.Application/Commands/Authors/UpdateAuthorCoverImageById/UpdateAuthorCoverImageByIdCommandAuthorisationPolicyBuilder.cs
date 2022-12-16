using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Authors.UpdateAuthorCoverImageById;

public sealed class UpdateAuthorCoverImageByIdCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<UpdateAuthorCoverImageByIdCommand>
{
    private readonly IAuthorReadOnlyRepository _repository;

    public UpdateAuthorCoverImageByIdCommandAuthorisationPolicyBuilder(IAuthorReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdateAuthorCoverImageByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.AuthorId);
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