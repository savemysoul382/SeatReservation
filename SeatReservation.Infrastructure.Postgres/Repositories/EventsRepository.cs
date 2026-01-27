// SeatReservation.Infrastructure.Postgres

using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservation.Application.EventsFolder;
using SeatReservation.Domain.Events;
using Shared;
using EventId = SeatReservation.Domain.Events.EventId;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class EventsRepository : IEventsRepository
{
    private readonly ReservationServiceDbContext _dbContext;
    private readonly ILogger<ReservationServiceDbContext> _logger;

    public EventsRepository(ReservationServiceDbContext dbContext, ILogger<ReservationServiceDbContext> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Event, Error>> GetById(EventId eventId, CancellationToken ct)
    {
        var @event = await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId, ct);
        if (@event == null)
        {
            return Error.Failure("event.not.found", "Event not found");
        }

        return @event;
    }

    // минус такого подхода, что бизнес логика начинает просачиваться в репозиторий
    public async Task<Result<Event, Error>> GetAvailableForReservationById(EventId eventId, CancellationToken ct)
    {
        var @event = await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId && e.StartDate > DateTime.UtcNow && e.Status == EventStatus.PLANNED, ct);
        if (@event == null)
        {
            return Error.Failure("event.not.found", "Event not found");
        }

        return @event;
    }
}