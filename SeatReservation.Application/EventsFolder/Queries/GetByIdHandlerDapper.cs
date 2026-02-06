// SeatReservation.Application

using Dapper;
using SeatReservation.Application.DataBase;
using SeatReservation.Contracts.Events;
using SeatReservation.Contracts.Seats;

namespace SeatReservation.Application.EventsFolder.Queries;

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
                COUNT(*) OVER () as total_seats,
                COUNT(rs.seat_id) OVER () as reserved_seats,
                COUNT(*) OVER () - COUNT(rs.seat_id) OVER () as available_seats
                s.id,
                s.venue_id,
                s.row_number,
                s.seat_number,
                rs is null is_available
            FROM events e
            JOIN events_details ed ON ed.event_id = e.id
            JOIN seats s ON e.venue_id = s.venue_id
            LEFT JOIN reservation_seats rs ON s.id = rs.seat_id AND rs.event_id = e.id
            LEFT JOIN reservation r ON rs.reservation_id = r.id
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