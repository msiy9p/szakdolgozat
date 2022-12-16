using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class BookAuthorConnectorConfiguration : BaseConfiguration,
    IEntityTypeConfiguration<BookAuthorConnector>
{
    public void Configure(EntityTypeBuilder<BookAuthorConnector> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(BookAuthorConnector)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.BookId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.AuthorId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.HasKey(x => new { x.BookId, x.AuthorId });

        // Relationships
        builder.HasOne(x => x.Book)
            .WithMany(x => x.BookAuthorConnectors)
            .HasForeignKey(x => x.BookId)
            .IsRequired();

        builder.HasOne(x => x.Author)
            .WithMany(x => x.BookAuthorConnectors)
            .HasForeignKey(x => x.AuthorId)
            .IsRequired();

        // Indexes
    }
}