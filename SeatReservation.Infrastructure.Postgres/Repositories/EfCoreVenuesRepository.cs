// SeatReservation.Infrastructure.Postgres

using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservation.Application.DataBase;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class EfCoreVenuesRepository : IVenuesRepository
{
    private readonly ReservationServiceDbContext _dbContext;
    private readonly ILogger<EfCoreVenuesRepository> _logger;

    public EfCoreVenuesRepository(ReservationServiceDbContext dbContext, ILogger<EfCoreVenuesRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
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
        // метод выполняется сразу, мы не пишем SaveChangesAsync
        int result = await _dbContext.Venues
            .Where(v => v.Id == venueId)
            .ExecuteUpdateAsync(
                setter => setter.SetProperty(
                    v => v.Name.Name,
                    venueName.Name),
                ct);

        return result > 0 ? venueId.Value : Error.Failure("venue.update", "Fail to update venue");
    }
}