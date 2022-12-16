using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Series.UpdateSeriesById;

public sealed class UpdateSeriesByIdCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<UpdateSeriesByIdCommand>
{
    private readonly ISeriesReadOnlyRepository _repository;

    public UpdateSeriesByIdCommandAuthorisationPolicyBuilder(ISeriesReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdateSeriesByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.SeriesId);
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