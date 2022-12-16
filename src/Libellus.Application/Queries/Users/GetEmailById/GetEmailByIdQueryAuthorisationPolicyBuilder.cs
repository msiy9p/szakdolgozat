using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Queries.Users.GetEmailById;

public sealed class
    GetEmailByIdQueryAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<GetEmailByIdQuery>
{
    public override void BuildPolicy(GetEmailByIdQuery instance)
    {
        UseRequirement(new IsCurrentUserRequirement(instance.UserId));
    }
}