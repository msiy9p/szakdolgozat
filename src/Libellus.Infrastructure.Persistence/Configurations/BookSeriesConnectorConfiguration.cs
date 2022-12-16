using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class BookSeriesConnectorConfiguration : BaseConfiguration,
    IEntityTypeConfiguration<BookSeriesConnector>
{
    public void Configure(EntityTypeBuilder<BookSeriesConnector> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(BookSeriesConnector)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.BookId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.SeriesId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.HasKey(x => new { x.BookId, x.SeriesId });

        builder.Property(x => x.NumberInSeries)
            .HasPrecision(5, 2)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.Series)
            .WithMany(x => x.BookSeriesConnectors)
            .HasForeignKey(x => x.SeriesId)
            .IsRequired();

        builder.HasOne(x => x.Book)
            .WithOne(x => x.BookSeriesConnector)
            .HasForeignKey<BookSeriesConnector>(x => x.BookId)
            .IsRequired();

        // Indexes
    }
}