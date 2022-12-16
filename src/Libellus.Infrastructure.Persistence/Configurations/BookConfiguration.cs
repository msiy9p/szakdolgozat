using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class BookConfiguration : BaseConfiguration, IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(Book)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.Id)
            .HasColumnName(Rewrite(nameof(Book) + nameof(Book.Id)))
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.FriendlyId)
            .HasColumnName(Rewrite(nameof(Book) + nameof(Book.FriendlyId)))
            .HasColumnOrder(NextColumnOrder())
            .HasMaxLength(BookFriendlyId.Length)
            .IsRequired();

        builder.HasKey(x => x.Id);

        builder.HasAlternateKey(x => x.FriendlyId);

        builder.Property(x => x.GroupId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.CreatorId)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.CoverImageId)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.LiteratureFormId)
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

        builder.Property(x => x.Description)
            .HasMaxLength(DescriptionLength)
            .HasColumnOrder(NextColumnOrder());

        builder.HasGeneratedTsVectorColumn(x => x.SearchVectorOne, CustomSearchLanguageOne.GetLanguageName(),
                x => new { x.Title, x.Description })
            .HasIndex(x => x.SearchVectorOne)
            .HasMethod(FtsIndexMethod);

        builder.HasGeneratedTsVectorColumn(x => x.SearchVectorTwo, CustomSearchLanguageTwo.GetLanguageName(),
                x => new { x.Title, x.Description })
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

        builder.HasOne(x => x.LiteratureForm)
            .WithMany(x => x.Books)
            .HasForeignKey(x => x.LiteratureFormId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(x => new { x.GroupId, x.Id, x.LiteratureFormId });

        builder.HasIndex(x => new { x.GroupId, x.Id, x.TitleNormalized });
    }
}