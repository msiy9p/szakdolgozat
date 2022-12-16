using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Libellus.Infrastructure.Persistence.DataModels.Contexts;

internal sealed class ApplicationContext : IdentityDbContext<ApplicationUser, ApplicationRole, UserId>
{
    internal readonly DomainEventContainer _domainEventContainer = new();

    internal DbSet<Comment> Comments => Set<Comment>();
    internal DbSet<Label> Labels => Set<Label>();
    internal DbSet<Group> Groups => Set<Group>();
    internal DbSet<GroupUserMembership> GroupUserMemberships => Set<GroupUserMembership>();
    internal DbSet<GroupRole> GroupRoles => Set<GroupRole>();
    internal DbSet<Post> Posts => Set<Post>();
    internal DbSet<LockedPost> LockedPosts => Set<LockedPost>();
    internal DbSet<Invitation> Invitations => Set<Invitation>();
    internal DbSet<InvitationRequest> InvitationRequests => Set<InvitationRequest>();

    internal DbSet<Author> Authors => Set<Author>();
    internal DbSet<BookAuthorConnector> BookAuthorConnectors => Set<BookAuthorConnector>();
    internal DbSet<BookGenreConnector> BookGenreConnectors => Set<BookGenreConnector>();
    internal DbSet<Book> Books => Set<Book>();
    internal DbSet<BookEdition> BookEditions => Set<BookEdition>();
    internal DbSet<BookSeriesConnector> BookSeriesConnectors => Set<BookSeriesConnector>();
    internal DbSet<BookTagConnector> BookTagConnectors => Set<BookTagConnector>();
    internal DbSet<BookWarningTagConnector> BookWarningTagConnectors => Set<BookWarningTagConnector>();
    internal DbSet<CoverImageMetaData> CoverImages => Set<CoverImageMetaData>();
    internal DbSet<Format> Formats => Set<Format>();
    internal DbSet<Genre> Genres => Set<Genre>();
    internal DbSet<Language> Languages => Set<Language>();
    internal DbSet<LiteratureForm> LiteratureForms => Set<LiteratureForm>();
    internal DbSet<Note> Notes => Set<Note>();
    internal DbSet<ProfilePictureMetaData> ProfilePictures => Set<ProfilePictureMetaData>();
    internal DbSet<Publisher> Publishers => Set<Publisher>();
    internal DbSet<Reading> Readings => Set<Reading>();
    internal DbSet<Series> Series => Set<Series>();
    internal DbSet<ShelfBookConnector> ShelfBookConnectors => Set<ShelfBookConnector>();
    internal DbSet<Shelf> Shelves => Set<Shelf>();
    internal DbSet<Tag> Tags => Set<Tag>();
    internal DbSet<WarningTag> WarningTags => Set<WarningTag>();

    public ApplicationContext() : base()
    {
    }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(BaseConfiguration.DefaultSchemaName);

        builder.Entity<ApplicationUser>()
            .ToTable(BaseConfiguration.Rewrite("AspNetUsers"), BaseConfiguration.SecuritySchemaName);
        builder.Entity<IdentityUserClaim<UserId>>().ToTable(BaseConfiguration.Rewrite("AspNetUserClaims"),
            BaseConfiguration.SecuritySchemaName);
        builder.Entity<IdentityUserLogin<UserId>>().ToTable(BaseConfiguration.Rewrite("AspNetUserLogins"),
            BaseConfiguration.SecuritySchemaName);
        builder.Entity<IdentityUserToken<UserId>>().ToTable(BaseConfiguration.Rewrite("AspNetUserTokens"),
            BaseConfiguration.SecuritySchemaName);

        builder.Entity<ApplicationRole>()
            .ToTable(BaseConfiguration.Rewrite("AspNetRoles"), BaseConfiguration.SecuritySchemaName);
        builder.Entity<IdentityRoleClaim<UserId>>().ToTable(BaseConfiguration.Rewrite("AspNetRoleClaims"),
            BaseConfiguration.SecuritySchemaName);
        builder.Entity<IdentityUserRole<UserId>>().ToTable(BaseConfiguration.Rewrite("AspNetUserRoles"),
            BaseConfiguration.SecuritySchemaName);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.AddIdConversions();
    }
}