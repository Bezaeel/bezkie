using bezkie.core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bezkie.infrastructure.Persistence.Configurations;

internal class RSVPConfiguration : IEntityTypeConfiguration<RSVP>
{
    public void Configure(EntityTypeBuilder<RSVP> builder)
    {
        builder.HasKey(x => new { x.BookId, x.CustomerId });

        builder.Property(x => x.Status).HasConversion<string>();

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.RSVPs)
            .HasForeignKey(x => x.CustomerId);
    }
}
