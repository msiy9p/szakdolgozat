using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.BookEditions.DeleteBookEditionCoverImageById;

public sealed class DeleteBookEditionCoverImageByIdCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<DeleteBookEditionCoverImageByIdCommand>
{
    private readonly IBookEditionReadOnlyRepository _repository;

    public DeleteBookEditionCoverImageByIdCommandAuthorisationPolicyBuilder(IBookEditionReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(DeleteBookEditionCoverImageByIdCommand instance)
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