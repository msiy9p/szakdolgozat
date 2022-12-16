using Libellus.Domain.Common.Types.Ids;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Libellus.Infrastructure.Persistence.DataModels.Contexts.Stores;

internal sealed class ApplicationRoleStore : RoleStore<ApplicationRole, ApplicationContext, UserId>
{
    public ApplicationRoleStore(ApplicationContext context, IdentityErrorDescriber? describer = null) : base(context,
        describer)
    {
        AutoSaveChanges = true;
    }

    public override async Task<ApplicationRole?> FindByIdAsync(string id,
        CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (Guid.TryParse(id, out var guidId))
        {
            var uid = new UserId(guidId);
            return await Context.Roles.FirstOrDefaultAsync(x => x.Id == uid, cancellationToken);
        }

        return null;
    }

    public async Task<ApplicationRole?> FindByIdAsync(UserId userId,
        CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return await Context.Roles.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }
}