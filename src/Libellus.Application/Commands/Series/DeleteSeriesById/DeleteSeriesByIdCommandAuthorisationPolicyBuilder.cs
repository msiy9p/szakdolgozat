using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Series.DeleteSeriesById;

public sealed class
    DeleteSeriesByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        DeleteSeriesByIdCommand>
{
    public override void BuildPolicy(DeleteSeriesByIdCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}