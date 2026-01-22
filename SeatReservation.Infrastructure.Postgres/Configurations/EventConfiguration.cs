// SeatReservation.Infrastructure.Postgres

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Domain.Events;

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
    }
}