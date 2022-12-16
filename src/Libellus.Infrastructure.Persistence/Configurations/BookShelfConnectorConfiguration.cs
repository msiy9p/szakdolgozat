using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class ShelfBookConnectorConfiguration : BaseConfiguration, IEntityTypeConfiguration<ShelfBookConnector>
{
    public void Configure(EntityTypeBuilder<ShelfBookConnector> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(ShelfBookConnector)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.ShelfId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.BookId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.HasKey(x => new { x.ShelfId, x.BookId });

        // Relationships
        builder.HasOne(x => x.Shelf)
            .WithMany(x => x.ShelfBookConnectors)
            .HasForeignKey(x => x.ShelfId)
            .IsRequired();

        builder.HasOne(x => x.Book)
            .WithMany(x => x.ShelfBookConnectors)
            .HasForeignKey(x => x.BookId)
            .IsRequired();

        // Indexes
    }
}