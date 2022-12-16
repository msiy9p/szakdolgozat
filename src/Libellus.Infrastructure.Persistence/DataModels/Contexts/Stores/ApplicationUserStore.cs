using Libellus.Domain.Common.Types.Ids;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Libellus.Infrastructure.Persistence.DataModels.Contexts.Stores;

internal sealed class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, ApplicationContext, UserId>
{
    public ApplicationUserStore(ApplicationContext context, IdentityErrorDescriber? describer = null) : base(context,
        describer)
    {
        AutoSaveChanges = true;
    }

    public override async Task<ApplicationUser?> FindByIdAsync(string userId,
        CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (Guid.TryParse(userId, out var guidId))
        {
            var id = new UserId(guidId);
            return await Context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        return null;
    }

    public async Task<ApplicationUser?> FindByIdAsync(UserId userId,
        CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return await Context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }
}