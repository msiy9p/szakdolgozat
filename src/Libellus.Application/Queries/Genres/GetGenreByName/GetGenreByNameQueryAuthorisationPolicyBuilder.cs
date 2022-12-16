using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Genres.GetGenreByName;

public sealed class
    GetGenreByNameQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetGenreByNameQuery>
{
    public override void BuildPolicy(GetGenreByNameQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}