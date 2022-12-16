using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.LiteratureForms.GetLiteratureFormByName;

public sealed class
    GetLiteratureFormByNameQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetLiteratureFormByNameQuery>
{
    public override void BuildPolicy(GetLiteratureFormByNameQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}