// SeatReservationService

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SeatReservation.Application.DataBase;
using SeatReservation.Contracts.Events;
using SeatReservation.Domain.Events;
using SeatReservation.Domain.Reservations;

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

        Expression<Func<Event, object>> keySelector = query.SortBy.ToLower() switch
        {
            "date" => e => e.EventDate,
            "name" => e => e.Name,
            "status" => e => e.Status,
            "type" => e => e.Type,
            "popularity" => e => (double)_readDbContext.ReservationSeatsRead
                .Count(rs => rs.EventId == e.Id && (rs.Reservation.Status == ReservationStatus.CANCELED
                                                    || rs.Reservation.Status == ReservationStatus.PENDING))
            / _readDbContext.SeatsRead.Count(s => s.VenueId == e.VenueId) * 100.0,
            _ => e => e.EventDate,
        };


        eventsQuery = query.SortDirection == "asc"
            ? eventsQuery.OrderBy(keySelector)
            : eventsQuery.OrderByDescending(keySelector);

        var totalCount = await eventsQuery.LongCountAsync(ct);

        eventsQuery = eventsQuery
            .Skip((query.Pagination.Page - 1) * query.Pagination.PageSize)
            .Take(query.Pagination.PageSize);

        var events = await eventsQuery
            .Include(e => e.Details)

            // V1
            .Select(e => new EventDto
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
                ReservedSeats = _readDbContext.ReservationSeatsRead.Count(rs => rs.EventId == e.Id &&
                                                                                (rs.Reservation.Status == ReservationStatus.CONFIRMED || rs.Reservation.Status == ReservationStatus.PENDING)),
                AvailableSeats = _readDbContext.SeatsRead.Count(s => s.VenueId == e.VenueId) -
                                 _readDbContext.ReservationSeatsRead.Count(rs => rs.EventId == e.Id &&
                                                                                 (rs.Reservation.Status == ReservationStatus.CONFIRMED ||
                                                                                  rs.Reservation.Status == ReservationStatus.PENDING)),
#pragma warning disable SA1413
                PopularityPercentage = Math.Round(
                    (double)_readDbContext.ReservationSeatsRead
                        .Count(rs => rs.EventId == e.Id && (rs.Reservation.Status == ReservationStatus.CANCELED
                                                            || rs.Reservation.Status == ReservationStatus.PENDING)) /
                    _readDbContext.SeatsRead.Count(s => s.VenueId == e.VenueId) * 100.0, 2),
#pragma warning restore SA1413
            })
            .ToListAsync(ct);

        return new GetEventsDto(events, totalCount);

        // V2 сортировка в памяти, но не можем из-за  невозможности сортировки по популярности, она не досутпна
        //    .Select(e => new
        //    {
        //        Event = e,
        //        TotalSeats = _readDbContext.SeatsRead.Count(s => s.VenueId == e.VenueId),
        //        ReservedSeats = _readDbContext.ReservationSeatsRead.Count(rs => rs.EventId == e.Id &&
        //                                                                        (rs.Reservation.Status == ReservationStatus.CONFIRMED || rs.Reservation.Status == ReservationStatus.PENDING)),
        //    })
        //    .ToListAsync(ct);

        // return new GetEventsDto(
        //    events.Select(e => new EventDto
        //    {
        //        Id = e.Event.Id.Value,
        //        Capacity = e.Event.Details.Capacity,
        //        Description = e.Event.Details.Description,
        //        LastReservationUtc = e.Event.Details.LastReservationUtc,
        //        VenueId = e.Event.VenueId.Value,
        //        Name = e.Event.Name,
        //        EventDate = e.Event.EventDate,
        //        StartDate = e.Event.StartDate,
        //        EndDate = e.Event.EndDate,
        //        Type = e.Event.Type.ToString(),
        //        Status = e.Event.Status.ToString(),
        //        Info = e.Event.Info.ToString(),
        //        TotalSeats = e.TotalSeats,
        //        ReservedSeats = e.ReservedSeats,
        //        AvailableSeats = e.TotalSeats - e.ReservedSeats,
        //        PopularityPercentage = e.TotalSeats == 0
        //            ? 0.0
        //            : Math.Round((double)e.ReservedSeats / e.TotalSeats * 100.0, 2),
        //    }).ToList(),
        //    totalCount);
    }
}