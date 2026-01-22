// SeatReservation.Infrastructure.Postgres

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Domain.Reservations;

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
    }
}