// SeatReservation.Infrastructure.Postgres

using CSharpFunctionalExtensions;
using SeatReservation.Domain.Events;
using Shared;

namespace SeatReservation.Application.EventsFolder;

public interface IEventsRepository
{
    Task<Result<Event, Error>> GetById(EventId eventId, CancellationToken ct);

    // Task<Result<Event, Error>> GetAvailableById(EventId eventId, CancellationToken ct);
}