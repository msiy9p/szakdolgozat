using Libellus.Application.Common.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Libellus.Infrastructure.Persistence.DataModels.Contexts.Initializers;

internal sealed class ApplicationContextInitializer
{
    private readonly ApplicationContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<ApplicationContextInitializer> _logger;

    public ApplicationContextInitializer(ApplicationContext context, UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager, ILogger<ApplicationContextInitializer> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task InitialiseAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_context.Database.IsNpgsql())
            {
                await _context.Database.MigrateAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await TrySeedAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync(CancellationToken cancellationToken = default)
    {
        // Identity roles
        var administratorRole = new ApplicationRole(SecurityConstants.IdentityRoles.Administrator);
        if (await _roleManager.Roles.AllAsync(x => x.Name != administratorRole.Name, cancellationToken))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        var userRole = new ApplicationRole(SecurityConstants.IdentityRoles.User);
        if (await _roleManager.Roles.AllAsync(x => x.Name != userRole.Name, cancellationToken))
        {
            await _roleManager.CreateAsync(userRole);
        }

        // Group roles
        var owner = new GroupRole(SecurityConstants.GroupRoles.Owner);
        if (await _context.GroupRoles.AllAsync(x => x.Name != owner.Name, cancellationToken))
        {
            await _context.GroupRoles.AddAsync(owner, cancellationToken);
        }

        var moderator = new GroupRole(SecurityConstants.GroupRoles.Moderator);
        if (await _context.GroupRoles.AllAsync(x => x.Name != moderator.Name, cancellationToken))
        {
            await _context.GroupRoles.AddAsync(moderator, cancellationToken);
        }

        var member = new GroupRole(SecurityConstants.GroupRoles.Member);
        if (await _context.GroupRoles.AllAsync(x => x.Name != member.Name, cancellationToken))
        {
            await _context.GroupRoles.AddAsync(member, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}