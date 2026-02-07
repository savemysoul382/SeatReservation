// SeatReservation.Infrastructure.Postgres

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Domain.Events;
using SeatReservation.Domain.Reservations;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class ReservationSeatConfiguration : IEntityTypeConfiguration<ReservationSeat>
{
    public void Configure(EntityTypeBuilder<ReservationSeat> builder)
    {
        builder.ToTable("reservation_seats");

        builder.HasKey(v => v.Id).HasName("pk_reservation_seats");

        builder.Property(v => v.Id)
            .HasConversion(v => v.Value, id => new ReservationSeatId(id))
            .HasColumnName("id");

        builder.Property(rs => rs.ReservedAt)
            .HasColumnName("reserved_at");

        builder
            .HasOne(rs => rs.Reservation)
            .WithMany(r => r.ReservedSeats)
            .HasForeignKey("reservation_id")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<Seat>()
            .WithMany()
            .HasForeignKey(rs => rs.SeatId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(rs => rs.SeatId)
            .HasConversion(r => r.Value, id => new SeatId(id))
            .HasColumnName("seat_id")
            .IsRequired();

        builder.Property(rs => rs.EventId)
            .HasConversion(r => r.Value, id => new EventId(id))
            .HasColumnName("event_id")
            .IsRequired();

        builder.HasIndex(rs => new
        {
            rs.EventId, rs.SeatId,
        }).IsUnique();

        // можно сделать индекс в табл. reservation_seats (reservation_id, seat_id)

        // создать в пустой миграции sql
        // CREATE INDEX idx_reservation_seats_event_id_reservation_id ON reservation_seats (event_id, reservation_id);
    }
}