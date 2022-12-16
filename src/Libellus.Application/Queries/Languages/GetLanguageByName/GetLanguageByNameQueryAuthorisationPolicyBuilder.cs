using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Languages.GetLanguageByName;

public sealed class
    GetLanguageByNameQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetLanguageByNameQuery>
{
    public override void BuildPolicy(GetLanguageByNameQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}