using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class CoverImageMetaDataConfiguration : BaseConfiguration, IEntityTypeConfiguration<CoverImageMetaData>
{
    public void Configure(EntityTypeBuilder<CoverImageMetaData> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(CoverImageMetaData)), MediaSchemaName);

        // Properties/Columns
        builder.Property(x => x.Id)
            .HasColumnName(Rewrite(nameof(CoverImageMetaData) + nameof(CoverImageMetaData.Id)))
            .HasColumnOrder(NextColumnOrder())
            .UseIdentityAlwaysColumn()
            .IsRequired();

        builder.Property(x => x.PublicId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.Width)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.Height)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.MimeType)
            .HasMaxLength(256)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.DataSize)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.CreatedOnUtc)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.ObjectName)
            .HasMaxLength(ObjectNameLength)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        // Relationships

        // Indexes
        builder.HasIndex(x => new { x.PublicId, x.Id });

        builder.HasIndex(x => x.ObjectName)
            .IsUnique();
    }
}