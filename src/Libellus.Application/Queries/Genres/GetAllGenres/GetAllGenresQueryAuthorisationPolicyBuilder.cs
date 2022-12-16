using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Genres.GetAllGenres;

public sealed class
    GetAllGenresQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetAllGenresQuery>
{
    public override void BuildPolicy(GetAllGenresQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}