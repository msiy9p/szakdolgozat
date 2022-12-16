using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class BookTagConnectorConfiguration : BaseConfiguration, IEntityTypeConfiguration<BookTagConnector>
{
    public void Configure(EntityTypeBuilder<BookTagConnector> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(BookTagConnector)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.BookId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.TagId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.HasKey(x => new { x.BookId, x.TagId });

        // Relationships
        builder.HasOne(x => x.Book)
            .WithMany(x => x.BookTagConnectors)
            .HasForeignKey(x => x.BookId)
            .IsRequired();

        builder.HasOne(x => x.Tag)
            .WithMany(x => x.BookTagConnectors)
            .HasForeignKey(x => x.TagId)
            .IsRequired();

        // Indexes
    }
}