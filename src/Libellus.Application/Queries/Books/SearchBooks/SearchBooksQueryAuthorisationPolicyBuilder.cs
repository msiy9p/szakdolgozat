using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Books.SearchBooks;

public sealed class
    SearchBooksQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<SearchBooksQuery>
{
    public override void BuildPolicy(SearchBooksQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}