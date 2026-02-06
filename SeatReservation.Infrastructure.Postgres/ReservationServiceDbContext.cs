using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservation.Application.DataBase;
using SeatReservation.Domain;
using SeatReservation.Domain.Events;
using SeatReservation.Domain.Reservations;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Infrastructure.Postgres
{
    public class ReservationServiceDbContext : DbContext, IReadDbContext
    {
        private readonly string _connectionString;

        public ReservationServiceDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString: _connectionString);

            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentLocation>(entity =>
            {
                entity.HasNoKey(); // разрешает отсутствие PK

                // entity.ToView("DepartmentLocations"); // опционально: привязка к VIEW
            });

            // modelBuilder.HasPostgresExtension("ltree");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationServiceDbContext).Assembly);
            modelBuilder.Entity<Venue>();
        }

        public DbSet<Venue> Venues => Set<Venue>();

        public DbSet<Seat> Seats => Set<Seat>();

        public DbSet<Reservation> Reservations => Set<Reservation>();

        public DbSet<ReservationSeat> ReservationSeats => Set<ReservationSeat>();

        public DbSet<Event> Events => Set<Event>();

        // только для чтения
        public IQueryable<Event> EventsRead => Set<Event>().AsQueryable().AsNoTracking();

        public IQueryable<Venue> VenuesRead => Set<Venue>().AsQueryable().AsNoTracking();

        public IQueryable<Seat> SeatsRead => Set<Seat>().AsQueryable().AsNoTracking();

        public IQueryable<Reservation> ReservationsRead => Set<Reservation>().AsQueryable().AsNoTracking();

        public IQueryable<ReservationSeat> ReservationSeatsRead => Set<ReservationSeat>().AsQueryable().AsNoTracking();

        private ILoggerFactory CreateLoggerFactory()
        {
            return LoggerFactory.Create(builder => { builder.AddConsole(); });
        }
    }

    // Example of using NpgsqlDataSource directly (not recommended in EF Core context)
    //    public class NpgSqlVenuesRepository
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