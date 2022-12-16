using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Books.UpdateBookCoverImageById;

public sealed class UpdateBookCoverImageByIdCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<UpdateBookCoverImageByIdCommand>
{
    private readonly IBookReadOnlyRepository _repository;

    public UpdateBookCoverImageByIdCommandAuthorisationPolicyBuilder(IBookReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdateBookCoverImageByIdCommand instance)
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