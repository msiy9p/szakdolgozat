using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class GroupUserMembershipConfiguration : BaseConfiguration,
    IEntityTypeConfiguration<GroupUserMembership>
{
    public void Configure(EntityTypeBuilder<GroupUserMembership> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(GroupUserMembership)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.GroupId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.HasKey(x => new { x.GroupId, x.UserId });

        builder.Property(x => x.GroupRoleId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.CreatedOnUtc)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.ModifiedOnUtc)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.Group)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.GroupId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.GroupRole)
            .WithMany(x => x.GroupUserMemberships)
            .HasForeignKey(x => x.GroupRoleId)
            .IsRequired();

        // Indexes
    }
}