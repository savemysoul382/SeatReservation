// SeatReservation.Application

using Dapper;
using Microsoft.EntityFrameworkCore;
using SeatReservation.Application.DataBase;
using SeatReservation.Contracts.Events;
using SeatReservation.Contracts.Seats;
using SeatReservation.Domain.Events;

namespace SeatReservation.Application.EventsFolder.Queries;

public class GetEventByIdHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetEventByIdHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<GetEventDto?> Handle(GetByIdRequest query, CancellationToken ct)
    {
        // V2
        var @event = await _readDbContext.EventsRead
           .Include(e => e.Details)
           .Where(e => e.Id == new EventId(query.EventId))
           .Select(@event =>
               new GetEventDto()
               {
                   Id = @event.Id.Value,
                   Capacity = @event.Details.Capacity,
                   Description = @event.Details.Description,
                   LastReservationUtc = @event.Details.LastReservationUtc,
                   VenueId = @event.VenueId.Value,
                   Name = @event.Name,
                   EventDate = @event.EventDate,
                   StartDate = @event.StartDate,
                   EndDate = @event.EndDate,
                   Type = @event.Type.ToString(),
                   Status = @event.Status.ToString(),
                   Info = @event.Info.ToString(),
                   Seats = (from s in _readDbContext.SeatsRead
                            //where s.VenueId == e.VenueId
                            join e in _readDbContext.EventsRead on s.VenueId equals @event.VenueId
                            join rs in _readDbContext.ReservationSeatsRead
                                on new { SeatId = s.Id, EventId = @event.Id } equals new { SeatId = rs.SeatId, EventId = rs.EventId }
                                into reservation
                            from r in reservation.DefaultIfEmpty()
                            where @event.Id == new EventId(query.EventId)
                            orderby s.RowNumber, s.SeatNumber
                            select new AvailableSeatDto
                            {
                                Id = s.Id.Value,
                                RowNumber = s.RowNumber,
                                SeatNumber = s.SeatNumber,
                                VenueId = s.VenueId.Value,
                                IsAvailable = r == null,
                            }).ToList(),
               })
           .FirstOrDefaultAsync(ct);

        return @event;

        // V3
        // var seats = await (from s in _readDbContext.SeatsRead
        //    join e in _readDbContext.EventsRead on s.VenueId equals e.VenueId
        //    join rs in _readDbContext.ReservationSeatsRead
        //        on new { SeatId = s.Id, EventId = e.Id } equals new { SeatId = rs.SeatId, EventId = rs.EventId }
        //        into reservation
        //    from r in reservation.DefaultIfEmpty()
        //    where e.Id == new EventId(query.EventId)
        //    orderby s.RowNumber, s.SeatNumber
        //    select new AvailableSeatDto
        //    {
        //        Id = s.Id.Value,
        //        RowNumber = s.RowNumber,
        //        SeatNumber = s.SeatNumber,
        //        VenueId = s.VenueId.Value,
        //        IsAvailable = r == null,
        //    }).ToListAsync(ct);

        // V1
        // var @event = await _readDbContext.EventsRead
        //    .Include(e => e.Details)
        //    .Where(e => e.Id == new EventId(query.EventId))
        //    .Select(e =>
        //        new GetEventDto()
        //        {
        //            Id = e.Id.Value,
        //            Capacity = e.Details.Capacity,
        //            Description = e.Details.Description,
        //            LastReservationUtc = e.Details.LastReservationUtc,
        //            VenueId = e.VenueId.Value,
        //            Name = e.Name,
        //            EventDate = e.EventDate,
        //            StartDate = e.StartDate,
        //            EndDate = e.EndDate,
        //            Type = e.Type.ToString(),
        //            Status = e.Status.ToString(),
        //            Info = e.Info.ToString(),
        //            Seats = _readDbContext.SeatsRead
        //                .Where(s => s.VenueId == e.VenueId)
        //                .OrderBy(s => s.RowNumber)
        //                .ThenBy(s => s.SeatNumber)
        //                .Select(s => new AvailableSeatDto()
        //                {
        //                    Id = s.Id.Value,
        //                    RowNumber = s.RowNumber,
        //                    SeatNumber = s.SeatNumber,
        //                    VenueId = s.VenueId.Value,
        //                    IsAvailable = !_readDbContext.ReservationSeatsRead
        //                        .Any(rs => rs.SeatId == s.Id && rs.EventId == e.Id),
        //                })
        //                .ToList(),
        //        })
        //    .FirstOrDefaultAsync(ct);
        // return @event;
    }
}

public class GetByIdHandlerDapper
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetByIdHandlerDapper(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GetEventDto?> Handle(GetByIdRequest query, CancellationToken ct)
    {
        var connection = await _connectionFactory.CreateConnectionAsync(ct);

        GetEventDto? getEventDto = null;

        var events = await connection.QueryAsync<GetEventDto, AvailableSeatDto, GetEventDto>(
            """
            SELECT 
                e.id,
                e.venue_id,
                e.name,
                e.type,
                e.event_date,
                e.start_date,
                e.end_date,
                e.status,
                e.info,
                ed.capacity,
                ed.description,
                s.id,
                s.venue_id,
                s.row_number,
                s.seat_number,
                rs is null is_available
            FROM events e
            JOIN events_details ed ON ed.event_id = e.id
            JOIN seats s ON e.venue_id = s.venue_id
            LEFT JOIN reservation_seats rs ON s.id = rs.seat_id AND rs.event_id = e.id
            WHERE  e.id = @eventId
            ORDER BY s.row_number, s.seat_number
            """,
            param: new { eventId = query.EventId },
            splitOn: "id",
            map: (eventDto, seatDto) =>
            {
                getEventDto ??= eventDto;
                getEventDto.Seats.Add(seatDto);

                return getEventDto;
            });

        return events.FirstOrDefault();
    }
}