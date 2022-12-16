using Libellus.Infrastructure.Persistence.Configurations.Common;
using Libellus.Infrastructure.Persistence.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;

namespace Libellus.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : BaseConfiguration, IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.ProfilePictureId);

        builder.Property<ZonedDateTime>(Rewrite("CreatedOnUtc"))
            .HasDefaultValueSql("now()")
            .IsRequired();
    }
}