using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class ReadingConfiguration : BaseConfiguration, IEntityTypeConfiguration<Reading>
{
    public void Configure(EntityTypeBuilder<Reading> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(Reading)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.Id)
            .HasColumnName(Rewrite(nameof(Reading) + nameof(Reading.Id)))
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.FriendlyId)
            .HasColumnName(Rewrite(nameof(Reading) + nameof(Reading.FriendlyId)))
            .HasColumnOrder(NextColumnOrder())
            .HasMaxLength(ReadingFriendlyId.Length)
            .IsRequired();

        builder.HasKey(x => x.Id);

        builder.HasAlternateKey(x => x.FriendlyId);

        builder.Property(x => x.GroupId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.CreatorId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.BookEditionId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.NoteId)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.IsDnf)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.IsReread)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.Score)
            .HasPrecision(7, 4)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.StartedOnUtc)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.FinishedOnUtc)
            .HasColumnOrder(NextColumnOrder());

        // Relationships
        builder.HasOne(x => x.Group)
            .WithMany()
            .HasForeignKey(x => x.GroupId)
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.CreatorId)
            .IsRequired();

        builder.HasOne(x => x.BookEdition)
            .WithMany(x => x.Readings)
            .HasForeignKey(x => x.BookEditionId)
            .IsRequired();

        builder.HasOne(x => x.Note)
            .WithOne(x => x.Reading)
            .HasForeignKey<Reading>(x => x.NoteId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
    }
}