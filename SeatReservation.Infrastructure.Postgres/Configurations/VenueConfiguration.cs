// SeatReservation.Infrastructure.Postgres

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Domain;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("venues");

        builder.HasKey(v => v.Id).HasName("pk_venues");

        builder.Property(v => v.Id)
            .HasConversion(v => v.Value, id => new VenueId(id))
            .HasColumnName("id");

        // builder.ComplexProperty();
        builder.ComplexProperty(
            v => v.Name,
            nb =>
            {
                // .IsRequired() тут можем не указывать, в домене настроено правило
                nb
                    .IsRequired()
                    .Property(v => v.Prefix)
                    .HasMaxLength(LengthConstants.LENGTH50).HasColumnName("prefix");

                // .IsRequired() тут можем не указывать, в домене настроено правило
                nb
                    .IsRequired()
                    .Property(v => v.Name)
                    .HasMaxLength(LengthConstants.LENGTH500).HasColumnName("name");
            });

        builder.HasMany(v => v.Seats)
            // .WithOne(s => s.Venue)
            .WithOne()
            .HasForeignKey(s => s.VenueId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}