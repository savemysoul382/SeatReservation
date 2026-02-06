// SeatReservation.Application

using Microsoft.EntityFrameworkCore;
using SeatReservation.Application.DataBase;
using SeatReservation.Contracts.Events;
using SeatReservation.Contracts.Seats;
using SeatReservation.Domain.Events;
using SeatReservation.Domain.Reservations;

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
                        where s.VenueId == @event.VenueId
                        // join e in _readDbContext.EventsRead on s.VenueId equals @event.VenueId
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
                    TotalSeats = _readDbContext.SeatsRead.Count(s => s.VenueId == @event.VenueId),
                    ReservedSeats = _readDbContext.ReservationSeatsRead.Count(rs => rs.EventId == @event.Id),
                    AvailableSeats = _readDbContext.SeatsRead.Count(s => s.VenueId == @event.VenueId) -
                                     _readDbContext.ReservationSeatsRead.Count(rs => rs.EventId == @event.Id &&
                                                                                     (rs.Reservation.Status == ReservationStatus.CONFIRMED ||
                                                                                      rs.Reservation.Status == ReservationStatus.PENDING)),
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