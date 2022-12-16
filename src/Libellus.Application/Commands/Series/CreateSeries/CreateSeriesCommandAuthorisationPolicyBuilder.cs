using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Series.CreateSeries;

public sealed class
    CreateSeriesCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<CreateSeriesCommand>
{
    public override void BuildPolicy(CreateSeriesCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);
    }
}