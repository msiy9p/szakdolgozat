using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Genres.UpdateGenreById;

public sealed class
    UpdateGenreByIdCommandAuthorisationPolicyBuilder :
        BaseAuthorisationPolicyBuilder<UpdateGenreByIdCommand>
{
    private readonly IGenreReadOnlyRepository _repository;

    public UpdateGenreByIdCommandAuthorisationPolicyBuilder(IGenreReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdateGenreByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.GenreId);
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