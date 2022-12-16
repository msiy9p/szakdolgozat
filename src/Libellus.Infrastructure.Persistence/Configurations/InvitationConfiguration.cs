using Libellus.Domain.Enums;
using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class InvitationConfiguration : BaseConfiguration, IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(Invitation)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.Id)
            .HasColumnName(Rewrite(nameof(Invitation) + nameof(Invitation.Id)))
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.GroupId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.InviterId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.InviteeId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.InvitationStatus)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.CreatedOnUtc)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.ModifiedOnUtc)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.Group)
            .WithMany()
            .HasForeignKey(x => x.GroupId)
            .IsRequired();

        builder.HasOne(x => x.Inviter)
            .WithMany()
            .HasForeignKey(x => x.InviterId)
            .IsRequired();

        builder.HasOne(x => x.Invitee)
            .WithMany()
            .HasForeignKey(x => x.InviteeId)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => new { x.GroupId, x.Id });
        builder.HasIndex(x => new { x.InviteeId, x.Id });

        builder.HasIndex(x => new { x.Id, x.InvitationStatus })
            .HasFilter($"{Rewrite(nameof(Invitation.InvitationStatus))} = {(int)InvitationStatus.Pending}")
            .IncludeProperties(x => x.CreatedOnUtc);
    }
}