// SeatReservationService

using Microsoft.EntityFrameworkCore;
using SeatReservation.Application.DataBase;
using SeatReservation.Contracts.Events;
using SeatReservation.Domain.Reservations;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Application.EventsFolder.Queries;

public class GetEventsHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetEventsHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<GetEventsDto> Handle(GetEventsRequest query, CancellationToken ct)
    {
        var eventsQuery = _readDbContext.EventsRead;

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            eventsQuery = eventsQuery.Where(e => EF.Functions.Like(e.Name.ToLower(), $"%{query.Search.ToLower()}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.EventType))
        {
            eventsQuery = eventsQuery.Where(e => e.Type.ToString().ToLower() == query.EventType.ToLower());
        }

        if (query.DateFrom.HasValue)
        {
            eventsQuery = eventsQuery.Where(e => e.EventDate >= query.DateFrom.Value.ToUniversalTime());
        }

        if (query.DateTo.HasValue)
        {
            eventsQuery = eventsQuery.Where(e => e.EventDate <= query.DateTo.Value.ToUniversalTime());
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            eventsQuery = eventsQuery.Where(e => e.Status.ToString().ToLower() == query.Status.ToLower());
        }

        if (query.VenueId.HasValue)
        {
            eventsQuery = eventsQuery.Where(e => e.VenueId.Value == query.VenueId);
        }

        if (query.MinAvailableSeats.HasValue)
        {
            eventsQuery = eventsQuery.Where(e =>
                _readDbContext.SeatsRead.Count(s => s.VenueId == e.VenueId) -
                _readDbContext.ReservationSeatsRead.Count(rs =>
                    rs.EventId == e.Id &&
                    (rs.Reservation.Status == ReservationStatus.CONFIRMED ||
                     rs.Reservation.Status == ReservationStatus.PENDING))
                >= query.MinAvailableSeats.Value);
        }

        eventsQuery = eventsQuery.OrderBy(e => e.EndDate);

        var totalCount = await eventsQuery.LongCountAsync(ct);

        eventsQuery = eventsQuery
            .Skip((query.Pagination.Page - 1) * query.Pagination.PageSize)
            .Take(query.Pagination.PageSize);

        var events = await eventsQuery
            .Select(e => new EvetDto
            {
                Id = e.Id.Value,
                Capacity = e.Details.Capacity,
                Description = e.Details.Description,
                LastReservationUtc = e.Details.LastReservationUtc,
                VenueId = e.VenueId.Value,
                Name = e.Name,
                EventDate = e.EventDate,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Type = e.Type.ToString(),
                Status = e.Status.ToString(),
                Info = e.Info.ToString(),
                TotalSeats = _readDbContext.SeatsRead.Count(s => s.VenueId == e.VenueId),
                ReservedSeats = _readDbContext.ReservationSeatsRead.Count(rs => rs.EventId == e.Id),
                AvailableSeats = _readDbContext.SeatsRead.Count(s => s.VenueId == e.VenueId) -
                                 _readDbContext.ReservationSeatsRead.Count(rs => rs.EventId == e.Id &&
                                                                                 (rs.Reservation.Status == ReservationStatus.CONFIRMED ||
                                                                                  rs.Reservation.Status == ReservationStatus.PENDING)),
            })
            .ToListAsync(ct);

        return new GetEventsDto(events, totalCount);
    }
}