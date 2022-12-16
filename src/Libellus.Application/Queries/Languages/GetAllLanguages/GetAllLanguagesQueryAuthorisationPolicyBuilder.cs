using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Languages.GetAllLanguages;

public sealed class
    GetAllLanguagesQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetAllLanguagesQuery>
{
    public override void BuildPolicy(GetAllLanguagesQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}