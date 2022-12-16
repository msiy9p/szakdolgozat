using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class SeriesConfiguration : BaseConfiguration, IEntityTypeConfiguration<Series>
{
    public void Configure(EntityTypeBuilder<Series> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(Series)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.Id)
            .HasColumnName(Rewrite(nameof(Series) + nameof(Series.Id)))
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.FriendlyId)
            .HasColumnName(Rewrite(nameof(Series) + nameof(Series.FriendlyId)))
            .HasColumnOrder(NextColumnOrder())
            .HasMaxLength(SeriesFriendlyId.Length)
            .IsRequired();

        builder.HasKey(x => x.Id);

        builder.HasAlternateKey(x => x.FriendlyId);

        builder.Property(x => x.GroupId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.CreatorId)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.CreatedOnUtc)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.ModifiedOnUtc)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(TitleLength)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.TitleNormalized)
            .HasMaxLength(TitleLength)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.HasGeneratedTsVectorColumn(x => x.SearchVectorOne, CustomSearchLanguageOne.GetLanguageName(),
                x => x.Title)
            .HasIndex(x => x.SearchVectorOne)
            .HasMethod(FtsIndexMethod);

        builder.HasGeneratedTsVectorColumn(x => x.SearchVectorTwo, CustomSearchLanguageTwo.GetLanguageName(),
                x => x.Title)
            .HasIndex(x => x.SearchVectorTwo)
            .HasMethod(FtsIndexMethod);

        // Relationships
        builder.HasOne(x => x.Group)
            .WithMany()
            .HasForeignKey(x => x.GroupId)
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.CreatorId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(x => new { x.GroupId, x.TitleNormalized });

        builder.HasIndex(x => x.TitleNormalized)
            .HasMethod("GIN")
            .IsTsVectorExpressionIndex("simple");
    }
}