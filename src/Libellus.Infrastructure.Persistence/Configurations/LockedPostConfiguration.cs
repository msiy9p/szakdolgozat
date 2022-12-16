using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class LockedPostConfiguration : BaseConfiguration, IEntityTypeConfiguration<LockedPost>
{
    public void Configure(EntityTypeBuilder<LockedPost> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(LockedPost)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.PostId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.HasKey(x => x.PostId);

        builder.Property(x => x.LockCreatorId)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.LockReason)
            .HasMaxLength(CommentLength)
            .HasColumnOrder(NextColumnOrder());

        builder.Property(x => x.CreatedOnUtc)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.Post)
            .WithOne(x => x.LockedPost)
            .HasForeignKey<LockedPost>(x => x.PostId)
            .IsRequired();

        builder.HasOne(x => x.LockCreator)
            .WithMany()
            .HasForeignKey(x => x.LockCreatorId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
    }
}