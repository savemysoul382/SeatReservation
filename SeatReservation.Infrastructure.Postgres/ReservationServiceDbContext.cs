using Microsoft.EntityFrameworkCore;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Infrastructure.Postgres
{
    public record VenueDto(Guid Id, string Name);

    public class ReservationServiceDbContext : DbContext
    {
        private readonly string _connectionString;

        public ReservationServiceDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder: optionsBuilder);

            optionsBuilder.UseNpgsql(connectionString: _connectionString);
        }

        public DbSet<Venue> Venues => Set<Venue>();
    }

    // Example of using NpgsqlDataSource directly (not recommended in EF Core context)
    //    public class VenueRepository
    //    {
    //        public async Task AddVenue(Venue venue)
    //        {
    //            var connectionString = "Server=localhost;Port=5434;Username=postgres;Password=postgres;Database=reservation_service_db;";
    //            await using var dataSource = NpgsqlDataSource.Create(connectionString);
    //            var sql = "INSERT INTO venues (id,name) VALUES (@id, @name) ";
    //            var command = dataSource.CreateCommand(sql);
    //            command.Parameters.Add(new NpgsqlParameter("id", venue.Id));
    //            command.Parameters.Add(new NpgsqlParameter("name", venue.Name));
    //            await command.ExecuteNonQueryAsync(CancellationToken.None);
    //         }
    //    }
    //    public async Task<List<VenueDto>> GetVenues(Venue venue)
    //    {
    //        var connectionString = "Server=localhost;Port=5434;Username=postgres;Password=postgres;Database=reservation_service_db;";
    //        await using var dataSource = NpgsqlDataSource.Create(connectionString: connectionString);
    //        var sql = """SELECT id, name FROM venues""";
    //        var command = dataSource.CreateCommand(commandText: sql);
    //        await using var reader = await command.ExecuteReaderAsync();
    //        var venues = new List<VenueDto>();
    //        while (await reader.ReadAsync())
    //        {
    //            venues.Add(new VenueDto(
    //                    reader.GetGuid(0),
    //                    reader.GetString(1)));
    //        }
    //        return venues;
    //    }
    // }
}