using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class BookEditionConfiguration : BaseConfiguration, IEntityTypeConfiguration<BookEdition>
{
    public void Configure(EntityTypeBuilder<BookEdition> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(BookEdition)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.Id)
            .HasColumnName(Rewrite(nameof(BookEdition) + nameof(BookEdition.Id)))
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.FriendlyId)
            .HasColumnName(Rewrite(nameof(BookEdition) + nameof(BookEdition.FriendlyId)))
            .HasColumnOrder(NextColumnOrder())
            .HasMaxLength(BookEditionFriendlyId.Length)
            .IsRequired();

        builder.HasKey(x => x.Id);

        builder.HasAlternateKey(x => x.FriendlyId);

        builder.Property(x => x.GroupId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.BookId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.CreatorId)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.CoverImageId)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.FormatId)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.LanguageId)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.PublisherId)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.PublishedOnYear)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.PublishedOnMonth)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.PublishedOnDay)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.PublishedOn)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.PageCount)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.WordCount)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.Isbn)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.IsTranslation)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

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

        builder.HasOne(x => x.Book)
            .WithMany(x => x.BookEditions)
            .HasForeignKey(x => x.BookId)
            .IsRequired();

        builder.HasOne(x => x.Format)
            .WithMany(x => x.BookEditions)
            .HasForeignKey(x => x.FormatId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Language)
            .WithMany(x => x.BookEditions)
            .HasForeignKey(x => x.LanguageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Publisher)
            .WithMany(x => x.BookEditions)
            .HasForeignKey(x => x.PublisherId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(x => new { x.GroupId, x.Id, x.FormatId });

        builder.HasIndex(x => new { x.GroupId, x.Id, x.LanguageId });

        builder.HasIndex(x => new { x.GroupId, x.Id, x.PublisherId });

        builder.HasIndex(x => new { x.GroupId, x.Id, x.TitleNormalized });

        builder.HasIndex(x => new { x.GroupId, x.Id, x.Isbn });

        builder.HasIndex(x => new { x.Id, x.PublishedOn });
    }
}