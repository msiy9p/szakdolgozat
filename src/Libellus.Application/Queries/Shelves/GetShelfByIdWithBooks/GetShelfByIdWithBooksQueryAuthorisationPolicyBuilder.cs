using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Shelves.GetShelfByIdWithBooks;

public sealed class
    GetShelfByIdWithBooksQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetShelfByIdWithBooksQuery>
{
    public override void BuildPolicy(GetShelfByIdWithBooksQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}