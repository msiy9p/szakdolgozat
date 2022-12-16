using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class LiteratureFormConfiguration : BaseConfiguration, IEntityTypeConfiguration<LiteratureForm>
{
    public void Configure(EntityTypeBuilder<LiteratureForm> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(LiteratureForm)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.Id)
            .HasColumnName(Rewrite(nameof(LiteratureForm) + nameof(LiteratureForm.Id)))
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

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

        builder.Property(x => x.ScoreMultiplier)
            .HasPrecision(3, 2)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(ShortNameLength)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.NameNormalized)
            .HasMaxLength(ShortNameLength)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.HasGeneratedTsVectorColumn(x => x.SearchVectorOne, CustomSearchLanguageOne.GetLanguageName(),
                x => x.Name)
            .HasIndex(x => x.SearchVectorOne)
            .HasMethod(FtsIndexMethod);

        builder.HasGeneratedTsVectorColumn(x => x.SearchVectorTwo, CustomSearchLanguageTwo.GetLanguageName(),
                x => x.Name)
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
        builder.HasIndex(x => new { x.GroupId, x.NameNormalized })
            .IsUnique();
    }
}