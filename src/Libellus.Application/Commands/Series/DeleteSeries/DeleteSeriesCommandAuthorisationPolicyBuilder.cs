using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Series.DeleteSeries;

public sealed class
    DeleteSeriesCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteSeriesCommand>
{
    public override void BuildPolicy(DeleteSeriesCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}