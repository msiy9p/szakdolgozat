using Libellus.Domain.Common.Types.Ids;
using Microsoft.AspNetCore.Identity;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class ApplicationUser : IdentityUser<UserId>
{
    public ProfilePictureId? ProfilePictureId { get; set; }

    public ApplicationUser()
    {
    }

    public ApplicationUser(string userName) : base(userName)
    {
    }
}