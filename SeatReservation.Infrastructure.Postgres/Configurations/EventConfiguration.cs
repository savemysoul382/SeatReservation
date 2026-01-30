// SeatReservation.Infrastructure.Postgres

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Domain.Events;
using SeatReservation.Domain.Venues;
using SeatReservation.Infrastructure.Postgres.Converters;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");

        builder.HasKey(v => v.Id).HasName("pk_events");

        builder.Property(v => v.Id)
            .HasConversion(v => v.Value, id => new EventId(id))
            .HasColumnName("id");

        // WithMany - типа events, но не пишем
        builder
            .HasOne<Venue>()
            .WithMany()
            .HasForeignKey(e => e.VenueId)
            .IsRequired()
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);

        builder.Property(e => e.VenueId).HasColumnName("venue_id");

        builder.HasOne<Venue>()
            .WithMany()
            .HasForeignKey(e => e.VenueId)
            .IsRequired()
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);

        builder.Property(e => e.Name)
            .HasColumnName("name");

        builder.Property(e => e.EventDate)
            .HasColumnName("event_date");

        builder.Property(e => e.StartDate)
            .HasColumnName("start_date");

        builder.Property(e => e.EndDate)
            .HasColumnName("end_date");

        builder.Property(e => e.Status)
            .HasColumnName("status");

        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasColumnName("type");

        builder.Property(e => e.Info)
            .HasConversion(new EventInfoConverter())
            .HasColumnName("info");
    }
}