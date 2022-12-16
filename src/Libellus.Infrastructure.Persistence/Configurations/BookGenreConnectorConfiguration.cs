using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class BookGenreConnectorConfiguration : BaseConfiguration, IEntityTypeConfiguration<BookGenreConnector>
{
    public void Configure(EntityTypeBuilder<BookGenreConnector> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(BookGenreConnector)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.BookId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.GenreId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.HasKey(x => new { x.BookId, x.GenreId });

        // Relationships
        builder.HasOne(x => x.Book)
            .WithMany(x => x.BookGenreConnectors)
            .HasForeignKey(x => x.BookId)
            .IsRequired();

        builder.HasOne(x => x.Genre)
            .WithMany(x => x.BookGenreConnectors)
            .HasForeignKey(x => x.GenreId)
            .IsRequired();

        // Indexes
    }
}