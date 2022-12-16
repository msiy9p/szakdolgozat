using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Languages.GetLanguageById;

public sealed class
    GetLanguageByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetLanguageByIdQuery>
{
    public override void BuildPolicy(GetLanguageByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}