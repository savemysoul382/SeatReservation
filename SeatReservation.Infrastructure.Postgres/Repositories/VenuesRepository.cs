// SeatReservation.Infrastructure.Postgres

using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservation.Application.DataBase;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class VenuesRepository : IVenuesRepository
{
    private readonly ReservationServiceDbContext _dbContext;
    private readonly ILogger<VenuesRepository> _logger;

    public VenuesRepository(ReservationServiceDbContext dbContext, ILogger<VenuesRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Venue, Error>> GetById(VenueId id, CancellationToken ct)
    {
        var venue = await _dbContext.Venues
            .FirstOrDefaultAsync(v => v.Id == id, ct);

        var entries = _dbContext.ChangeTracker.Entries();

        return venue != null
            ? venue
            : Error.NotFound("Venue not found", id.Value);
    }

    public async Task<Result<Venue, Error>> GetByIdWithSeats(VenueId id, CancellationToken ct)
    {
        var venue = await _dbContext.Venues
            .Include(v => v.Seats)
            .FirstOrDefaultAsync(v => v.Id == id, ct);

        var entries = _dbContext.ChangeTracker.Entries();

        return venue != null
            ? venue
            : Error.NotFound("Venue not found", id.Value);
    }

    public async Task<IReadOnlyList<Venue>> GetByPrefix(string prefix, CancellationToken ct)
    {
        var venues = await _dbContext.Venues
            .Where(v => v.Name.Prefix.StartsWith(prefix))
            .ToListAsync<Venue>(cancellationToken: ct);

        return venues;
    }

    // еще один способ, но он не очень приветствуется
    public async Task Update(Venue venue)
    {
        var entries = _dbContext.ChangeTracker.Entries();

        _dbContext.Venues.Update(venue);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<Result<Guid, Error>> Add(Venue venue, CancellationToken ct)
    {
        try
        {
            await _dbContext.Venues.AddAsync(venue, ct);
            await _dbContext.SaveChangesAsync(ct);
            return venue.Id.Value;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Fail to insert venue with id: {Id}", venue.Id.Value);
            return Error.Failure("venue.insert", "Fail to insert venue");
        }
    }

    public async Task<Result<Guid, Error>> UpdateVenueName(VenueId venueId, VenueName venueName, CancellationToken ct)
    {
        // Dapper approach
        // DbConnection connection = _dbContext.Database.GetDbConnection();
        // connection.ExecuteAsync()
        await _dbContext.Database.ExecuteSqlAsync($"UPDATE venues SET name = '{venueName.Name}' WHERE id = {venueId.Value}", cancellationToken: ct);

        // RawSql approach with parameters
        // await _dbContext.Database.ExecuteSqlRawAsync(
        //    $"UPDATE venues SET name = @Name WHERE id = @Id",
        //    new NpgsqlParameter[] { new NpgsqlParameter("@Name", venueName.Name), new NpgsqlParameter("@Id", venueId.Value),},
        //    cancellationToken: ct);

        // метод выполняется сразу, мы не пишем SaveChangesAsync
        // int result = await _dbContext.Venues
        //    .Where(v => v.Id == venueId)
        //    .ExecuteUpdateAsync(
        //        setter => setter
        //            .SetProperty(v => v.Name.Name, venueName.Name),
        //        ct);
        return venueId.Value;
    }

    public async Task<UnitResult<Error>> UpdateVenueNameByPrefix(string prefix, VenueName venueName, CancellationToken ct)
    {
        // метод выполняется сразу, мы не пишем SaveChangesAsync
        int result = await _dbContext.Venues
            .Where(v => v.Name.Prefix.StartsWith(prefix))
            .ExecuteUpdateAsync(
                setter => setter
                    .SetProperty(v => v.Name.Name, venueName.Name),
                ct);

        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> DeleteSeatsByVenueId(VenueId venueId, CancellationToken ct)
    {
        // метод выполняется сразу, мы не пишем SaveChangesAsync
        int result = await _dbContext.Seats
            .Where(s => s.Venue.Id == venueId)
            .ExecuteDeleteAsync(ct);

        return UnitResult.Success<Error>();
    }
}