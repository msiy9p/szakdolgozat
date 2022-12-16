using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class WarningTagConfiguration : BaseConfiguration, IEntityTypeConfiguration<WarningTag>
{
    public void Configure(EntityTypeBuilder<WarningTag> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(WarningTag)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.Id)
            .HasColumnName(Rewrite(nameof(WarningTag) + nameof(WarningTag.Id)))
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