using Libellus.Domain.Common.Types.Ids;
using Microsoft.AspNetCore.Identity;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class ApplicationRole : IdentityRole<UserId>
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }
}