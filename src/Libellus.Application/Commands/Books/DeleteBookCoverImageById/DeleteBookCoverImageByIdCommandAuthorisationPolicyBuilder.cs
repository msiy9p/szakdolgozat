using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Books.DeleteBookCoverImageById;

public sealed class DeleteBookCoverImageByIdCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<DeleteBookCoverImageByIdCommand>
{
    private readonly IBookReadOnlyRepository _repository;

    public DeleteBookCoverImageByIdCommandAuthorisationPolicyBuilder(IBookReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(DeleteBookCoverImageByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.BookId);
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