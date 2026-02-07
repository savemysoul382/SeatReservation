// SeatReservation.Infrastructure.Postgres

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("seats");

        builder.HasKey(v => v.Id).HasName("pk_seats");

        builder.Property(v => v.Id)
            .HasConversion(v => v.Value, id => new SeatId(id))
            .HasColumnName("id");

        builder.Property(v => v.VenueId)
            .HasConversion(v => v.Value, id => new VenueId(id))
            .HasColumnName("venue_id");

        // builder.Property(s => s.Id).HasColumnName("venue_id");
        builder.Property(v => v.RowNumber)
           .IsRequired()
           .HasColumnName("row_number");

        builder.Property(v => v.SeatNumber)
           .IsRequired()
           .HasColumnName("seat_number");

        builder.HasIndex(s => new
        {
            s.VenueId, s.RowNumber, s.SeatNumber,
        });

        // с фильтрами. Если делать с ComplexProperty - то так не получиться сделать.
        // Нужно делать пустую миграцию и в up-dawn писать вручную sql на создание индекса с фильтром.
        // migrationBuilder.Sql("CREATE INDEX IF NOT EXIST idx_seats_venue_id_row_number_seat_number ON seats(event_id, row_id, seat_id);")
        // в down написать DROP INDEX...
        // builder.HasIndex(s => new
        // {
        //    s.VenueId,
        //    s.RowNumber,
        //    s.SeatNumber,
        // }).HasFilter("row_number > 0 AND seat_number > 0");
    }
}