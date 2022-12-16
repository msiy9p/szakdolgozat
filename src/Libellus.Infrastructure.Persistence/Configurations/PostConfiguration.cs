using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class PostConfiguration : BaseConfiguration, IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(Post)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.Id)
            .HasColumnName(Rewrite(nameof(Post) + nameof(Post.Id)))
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.FriendlyId)
            .HasColumnName(Rewrite(nameof(Post) + nameof(Post.FriendlyId)))
            .HasColumnOrder(NextColumnOrder())
            .HasMaxLength(PostFriendlyId.Length)
            .IsRequired();

        builder.HasKey(x => x.Id);

        builder.HasAlternateKey(x => x.FriendlyId);

        builder.Property(x => x.GroupId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.CreatorId)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.LabelId)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.IsMemberOnly)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.IsSpoiler)
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

        builder.Property(x => x.Text)
            .HasMaxLength(CommentLength)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.HasGeneratedTsVectorColumn(x => x.SearchVectorOne, CustomSearchLanguageOne.GetLanguageName(),
                x => new { x.Title, x.Text })
            .HasIndex(x => x.SearchVectorOne)
            .HasMethod(FtsIndexMethod);

        builder.HasGeneratedTsVectorColumn(x => x.SearchVectorTwo, CustomSearchLanguageTwo.GetLanguageName(),
                x => new { x.Title, x.Text })
            .HasIndex(x => x.SearchVectorTwo)
            .HasMethod(FtsIndexMethod);

        // Relationships
        builder.HasOne(x => x.Group)
            .WithMany(x => x.Posts)
            .HasForeignKey(x => x.GroupId)
            .IsRequired();

        builder.HasOne(x => x.Creator)
            .WithMany()
            .HasForeignKey(x => x.CreatorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Label)
            .WithMany(x => x.Posts)
            .HasForeignKey(x => x.LabelId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(x => new { x.GroupId, x.Id, x.LabelId });

        builder.HasIndex(x => new { x.GroupId, x.TitleNormalized });

        builder.HasIndex(x => x.TitleNormalized)
            .HasMethod("GIN")
            .IsTsVectorExpressionIndex("simple");
    }
}