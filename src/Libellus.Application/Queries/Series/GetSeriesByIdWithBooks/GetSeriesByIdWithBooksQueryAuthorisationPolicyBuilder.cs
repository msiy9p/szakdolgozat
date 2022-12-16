using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Series.GetSeriesByIdWithBooks;

public sealed class
    GetSeriesByIdWithBooksQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        GetSeriesByIdWithBooksQuery>
{
    public override void BuildPolicy(GetSeriesByIdWithBooksQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}