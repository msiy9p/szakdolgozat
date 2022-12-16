using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Authors.UpdateAuthorById;

public sealed class UpdateAuthorByIdCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<UpdateAuthorByIdCommand>
{
    private readonly IAuthorReadOnlyRepository _repository;

    public UpdateAuthorByIdCommandAuthorisationPolicyBuilder(IAuthorReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdateAuthorByIdCommand instance)
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