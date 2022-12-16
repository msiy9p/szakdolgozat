using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Books.GetAllBooks;

public sealed class
    GetAllBooksQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetAllBooksQuery>
{
    public override void BuildPolicy(GetAllBooksQuery instance)
    {
        UseRequirement(CurrentUserCanViewCurrentGroupRequirement.Instance);
    }
}