// SeatReservation.Infrastructure.Postgres

namespace SeatReservation.Infrastructure.Postgres.Seeding;

public interface ISeeder
{
    Task SeedAsync();
}