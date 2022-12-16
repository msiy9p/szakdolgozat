using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Notes.GetNoteById;

public sealed class
    GetNoteByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetNoteByIdQuery>
{
    public override void BuildPolicy(GetNoteByIdQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}