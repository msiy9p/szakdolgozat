using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.BookEditions.UpdateBookEditionById;

public sealed class UpdateBookEditionByIdCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<UpdateBookEditionByIdCommand>
{
    private readonly IBookEditionReadOnlyRepository _repository;

    public UpdateBookEditionByIdCommandAuthorisationPolicyBuilder(IBookEditionReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdateBookEditionByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.BookEditionId);
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