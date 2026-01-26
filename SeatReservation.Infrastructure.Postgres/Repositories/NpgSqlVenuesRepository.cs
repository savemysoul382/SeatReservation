#pragma warning disable SA1005
//// SeatReservation.Infrastructure.Postgres

//using CSharpFunctionalExtensions;
//using Dapper;
//using Microsoft.Extensions.Logging;
//using SeatReservation.Application.DataBase;
//using SeatReservation.Domain.Venues;
//using SeatReservation.Infrastructure.Postgres.Database;
//using Shared;
//using Venue = SeatReservation.Domain.Venues.Venue;

//namespace SeatReservation.Infrastructure.Postgres.Repositories;

//public class NpgSqlVenuesRepository : IVenuesRepository
//{
//    private readonly IDbConnectionFactory _connectionFactory;
//    private readonly ILogger<NpgSqlVenuesRepository> _logger;

//    public NpgSqlVenuesRepository(IDbConnectionFactory connectionFactory, ILogger<NpgSqlVenuesRepository> logger)
//    {
//        _connectionFactory = connectionFactory;
//        _logger = logger;
//    }

//    public async Task<Result<Venue, Error>> GetById(VenueId id, CancellationToken ct)
//    {
//        using var connection = await _connectionFactory.CreateConnectionAsync(ct);

//        const string get_venue_sql = """
//                                     SELECT *
//                                     FROM venues
//                                     WHERE Id = @Id
//                                     """;

//        var updateNameSqlParams = new {
//            Id = id.Value,};

//        Venue? venue = await connection.QueryFirstOrDefaultAsync<Venue>(sql: get_venue_sql, param: updateNameSqlParams);
//        if (venue is null)
//        {
//            _logger.LogWarning("Venue with id: {Id} not found", id.Value);
//            return Error.NotFound("Venue not found", id.Value);
//        }

//        return venue;
//    }

//    public async Task<Result<Venue, Error>> GetByIdWithSeats(VenueId id, CancellationToken ct)
//    {
//        throw new NotImplementedException();
//    }

//    public async Task<IReadOnlyList<Venue>> GetByPrefix(string prefix, CancellationToken ct)
//    {
//        using var connection = await _connectionFactory.CreateConnectionAsync(ct);

//        const string get_venues_sql = """
//                                       SELECT *
//                                       FROM venues
//                                       WHERE prefix = @Prefix
//                                       """;
//        var queryParam = new
//        {
//            Prefix = prefix,
//        };

//        var venues = await connection.QueryAsync<Venue>(sql: get_venues_sql, param: queryParam);
//        return venues.ToList();
//    }

//    public async Task<Result<Guid, Error>> Add(Venue venue, CancellationToken ct)
//    {
//        using var connection = await _connectionFactory.CreateConnectionAsync(ct);

//        using var transaction = connection.BeginTransaction();

//        try
//        {
//            const string insert_venue_sql = """
//                                            INSERT INTO venues (id, prefix, name, "SeatsLimit")
//                                            VALUES (@Id, @Prefix, @Name, @SeatsLimit)
//                                            """;

//            var venueInsertParams = new
//            {
//                Id = venue.Id.Value, Prefix = venue.Name.Prefix, Name = venue.Name.Name, SeatsLimit = venue.SeatsLimit,
//            };

//            await connection.ExecuteAsync(sql: insert_venue_sql, param: venueInsertParams);

//            if (!venue.Seats.Any())
//            {
//                return venue.Id.Value;
//            }

//            const string seats_insert_sql = """
//                                            INSERT INTO seats (id, row_number, seat_number, venue_id)
//                                            VALUES (@Id, @RowNumber, @SeatNumber, @VenueId)
//                                            """;

//            var seats_insert_params = venue.Seats.Select(s => new
//            {
//                Id = s.Id.Value, RowNumber = s.RowNumber, SeatNumber = s.SeatNumber, VenueId = venue.Id.Value,
//            });

//            await connection.ExecuteAsync(sql: seats_insert_sql, param: seats_insert_params);

//            transaction.Commit();

//            return venue.Id.Value;
//        }
//        catch (Exception e)
//        {
//            transaction.Rollback();

//            _logger.LogError(e, "Fail to insert venue with id: {Id}", venue.Id.Value);

//            return Error.Failure("venue.insert", "Fail to insert venue");
//        }
//    }

//    public async Task Save()
//    {
//        throw new NotImplementedException();
//    }

//    public async Task Update(Venue venue)
//    {
//        throw new NotImplementedException();
//    }

//    public async Task<Result<Guid, Error>> UpdateVenueName(VenueId venueId, VenueName venueName, CancellationToken ct)
//    {
//        using var connection = await _connectionFactory.CreateConnectionAsync(ct);

//        using var transaction = connection.BeginTransaction();

//        try
//        {
//            const string update_name_sql = """
//                                           UPDATE venues
//                                           SET name = @Name
//                                           WHERE Id = @Id
//                                           """;

//            var updateNameSqlParams = new {Id = venueId.Value, Name = venueName.Name,};

//            await connection.ExecuteAsync(sql: update_name_sql, param: updateNameSqlParams);

//            transaction.Commit();

//            return venueId.Value;
//        }
//        catch (Exception e)
//        {
//            transaction.Rollback();

//            _logger.LogError(e, "Fail to update venue with venueId: {Id}", venueId.Value);

//            return Error.Failure("venue.update", "Fail to update venue");
//        }
//    }

//    public async Task<UnitResult<Error>> UpdateVenueNameByPrefix(string prefix, VenueName venueName, CancellationToken ct)
//    {
//        using var connection = await _connectionFactory.CreateConnectionAsync(ct);

//        using var transaction = connection.BeginTransaction();

//        try
//        {
//            const string update_name_sql = """
//                                           UPDATE venues
//                                           SET name = @Name
//                                           WHERE Prefix LIKE @Prefix
//                                           """;

//            var updateNameSqlParams = new {Prefix = $"{prefix}%", Name = venueName.Name,};

//            await connection.ExecuteAsync(sql: update_name_sql, param: updateNameSqlParams);

//            transaction.Commit();

//            return UnitResult.Success<Error>();
//        }
//        catch (Exception e)
//        {
//            transaction.Rollback();

//            _logger.LogError(e, "Fail to update venue with prefix: {prefix}", prefix);

//            return Error.Failure("venue.update", "Fail to update venue");
//        }
//    }

//    public async Task<UnitResult<Error>> DeleteSeatsByVenueId(VenueId venueId, CancellationToken ct)
//    {
//        throw new NotImplementedException();
//    }

//    public async Task<UnitResult<Error>> AddSeats(IEnumerable<Seat> seats, CancellationToken ct)
//    {
//        throw new NotImplementedException();
//    }
//}