using Libellus.Domain.Enums;
using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class InvitationRequestRequestConfiguration : BaseConfiguration,
    IEntityTypeConfiguration<InvitationRequest>
{
    public void Configure(EntityTypeBuilder<InvitationRequest> builder)
    {
        // Table
        builder.ToTable(Rewrite(nameof(InvitationRequest)), SocialSchemaName);

        // Properties/Columns
        builder.Property(x => x.Id)
            .HasColumnName(Rewrite(nameof(InvitationRequest) + nameof(InvitationRequest.Id)))
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.GroupId)
            .HasColumnOrder(NextColumnOrder())
            .IsRequired();

        builder.Property(x => x.RequesterId)
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

        builder.HasOne(x => x.Requester)
            .WithMany()
            .HasForeignKey(x => x.RequesterId)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => new { x.GroupId, x.Id });

        builder.HasIndex(x => new { x.Id, x.InvitationStatus })
            .HasFilter($"{Rewrite(nameof(InvitationRequest.InvitationStatus))} = {(int)InvitationStatus.Pending}")
            .IncludeProperties(x => x.CreatedOnUtc);
    }
}