using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class BookWarningTagConnectorConfiguration : BaseConfiguration,
    IEntityTypeConfiguration<BookWarningTagConnector>
{
    public void Configure(EntityTypeBuilder<BookWarningTagConnector> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(BookWarningTagConnector)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.BookId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.WarningTagId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.HasKey(x => new { x.BookId, x.WarningTagId });

        // Relationships
        builder.HasOne(x => x.Book)
            .WithMany(x => x.BookWarningTagConnectors)
            .HasForeignKey(x => x.BookId)
            .IsRequired();

        builder.HasOne(x => x.WarningTag)
            .WithMany(x => x.BookWarningTagConnectors)
            .HasForeignKey(x => x.WarningTagId)
            .IsRequired();

        // Indexes
    }
}