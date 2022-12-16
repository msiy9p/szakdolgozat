using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.LiteratureForms.GetAllLiteratureForms;

public sealed class
    GetAllLiteratureFormsQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetAllLiteratureFormsQuery>
{
    public override void BuildPolicy(GetAllLiteratureFormsQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}