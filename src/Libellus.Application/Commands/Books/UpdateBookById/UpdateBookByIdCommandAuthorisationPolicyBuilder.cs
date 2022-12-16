using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Books.UpdateBookById;

public sealed class UpdateBookByIdCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<UpdateBookByIdCommand>
{
    private readonly IBookReadOnlyRepository _repository;

    public UpdateBookByIdCommandAuthorisationPolicyBuilder(IBookReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdateBookByIdCommand instance)
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