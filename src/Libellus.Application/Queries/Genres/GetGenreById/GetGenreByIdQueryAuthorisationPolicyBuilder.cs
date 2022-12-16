using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Genres.GetGenreById;

public sealed class
    GetGenreByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetGenreByIdQuery>
{
    public override void BuildPolicy(GetGenreByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}