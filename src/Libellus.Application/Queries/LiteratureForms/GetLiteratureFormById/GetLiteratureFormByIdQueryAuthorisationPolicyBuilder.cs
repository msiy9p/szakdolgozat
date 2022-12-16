using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.LiteratureForms.GetLiteratureFormById;

public sealed class
    GetLiteratureFormByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetLiteratureFormByIdQuery>
{
    public override void BuildPolicy(GetLiteratureFormByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}