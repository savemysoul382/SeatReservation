// SeatReservation.Infrastructure.Postgres

using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservation.Application.Seats;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class SeatsRepository : ISeatsRepository
{
    private readonly ReservationServiceDbContext _dbContext;
    private readonly ILogger<SeatsRepository> _logger;

    public SeatsRepository(ReservationServiceDbContext dbContext, ILogger<SeatsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Seat>> GetByIds(IEnumerable<SeatId> seatIds, CancellationToken ct)
    {
        var seats = await _dbContext.Seats.Where(s => seatIds.Contains(s.Id)).ToListAsync(ct);
        return seats;
    }
}