using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Authors.DeleteAuthorCoverImageById;

public sealed class DeleteAuthorCoverImageByIdCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<DeleteAuthorCoverImageByIdCommand>
{
    private readonly IAuthorReadOnlyRepository _repository;

    public DeleteAuthorCoverImageByIdCommandAuthorisationPolicyBuilder(IAuthorReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(DeleteAuthorCoverImageByIdCommand instance)
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