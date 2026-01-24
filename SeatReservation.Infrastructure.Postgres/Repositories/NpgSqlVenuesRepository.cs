// SeatReservation.Infrastructure.Postgres

using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using SeatReservation.Application.DataBase;
using SeatReservation.Domain.Venues;
using SeatReservation.Infrastructure.Postgres.Database;
using Shared;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class NpgSqlVenuesRepository : IVenuesRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<NpgSqlVenuesRepository> _logger;

    public NpgSqlVenuesRepository(IDbConnectionFactory connectionFactory, ILogger<NpgSqlVenuesRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> Add(Venue venue, CancellationToken ct)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync(ct);

        using var transaction = connection.BeginTransaction();

        try
        {
            const string insert_venue_sql = """
                                            INSERT INTO venues (id, prefix, name, "SeatsLimit") 
                                            VALUES (@Id, @Prefix, @Name, @SeatsLimit) 
                                            """;

            var venueInsertParams = new
            {
                Id = venue.Id.Value, Prefix = venue.Name.Prefix, Name = venue.Name.Name, SeatsLimit = venue.SeatsLimit,
            };

            await connection.ExecuteAsync(sql: insert_venue_sql, param: venueInsertParams);

            if (!venue.Seats.Any())
            {
                return venue.Id.Value;
            }

            const string seats_insert_sql = """
                                            INSERT INTO seats (id, row_number, seat_number, venue_id) 
                                            VALUES (@Id, @RowNumber, @SeatNumber, @VenueId) 
                                            """;

            var seats_insert_params = venue.Seats.Select(s => new
            {
                Id = s.Id.Value, RowNumber = s.RowNumber, SeatNumber = s.SeatNumber, VenueId = venue.Id.Value,
            });

            await connection.ExecuteAsync(sql: seats_insert_sql, param: seats_insert_params);

            transaction.Commit();

            return venue.Id.Value;
        }
        catch (Exception e)
        {
            transaction.Rollback();

            _logger.LogError(e, "Fail to insert venue with id: {VenueId}", venue.Id.Value);

            return Error.Failure("venue.insert", "Fail to insert venue");
        }
    }
}