using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class GroupRoleConfiguration : BaseConfiguration,
    IEntityTypeConfiguration<GroupRole>
{
    public void Configure(EntityTypeBuilder<GroupRole> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(GroupRole)), SecuritySchemaName);

        // Properties/Columns
        builder.Property(x => x.GroupRoleId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.HasKey(x => x.GroupRoleId);

        builder.Property(x => x.Name)
            .HasColumnOrder(NextColumnOrder())
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.NameNormalized)
            .HasColumnOrder(NextColumnOrder())
            .HasMaxLength(256)
            .IsRequired();

        // Relationships

        // Indexes
        builder.HasIndex(x => new { x.GroupRoleId, x.Name });

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}