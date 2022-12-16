using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Series.UpdateSeries;

public sealed class
    UpdateSeriesCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdateSeriesCommand>
{
    public override void BuildPolicy(UpdateSeriesCommand instance)
    {
        UseRequirement(new SameCreatorOrAboveRequirement(instance.Item.CreatorId));
    }
}