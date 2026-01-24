// SeatReservation.Infrastructure.Postgres

using CSharpFunctionalExtensions;
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
            _logger.LogError(e, "Fail to insert venue with id: {VenueId}", venue.Id.Value);
            return Error.Failure("venue.insert", "Fail to insert venue");
        }
    }
}