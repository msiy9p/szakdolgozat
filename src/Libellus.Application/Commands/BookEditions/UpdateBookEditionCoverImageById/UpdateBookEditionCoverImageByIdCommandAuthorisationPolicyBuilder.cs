using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.BookEditions.UpdateBookEditionCoverImageById;

public sealed class UpdateBookEditionCoverImageByIdCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<UpdateBookEditionCoverImageByIdCommand>
{
    private readonly IBookEditionReadOnlyRepository _repository;

    public UpdateBookEditionCoverImageByIdCommandAuthorisationPolicyBuilder(IBookEditionReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdateBookEditionCoverImageByIdCommand instance)
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