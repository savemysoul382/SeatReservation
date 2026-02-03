// SeatReservation.Application

using System.Data;
using Dapper;
using SeatReservation.Application.DataBase;
using SeatReservation.Contracts.Events;

namespace SeatReservation.Application.EventsFolder.Queries;

public class GetEventsHandlerDapper
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetEventsHandlerDapper(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GetEventsDto> Handle(GetEventsRequest query, CancellationToken ct)
    {
        var connection = await _connectionFactory.CreateConnectionAsync(ct);

        var parameters = new DynamicParameters();

        // dynamic conditions
        var conditions = new List<string>();
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            conditions.Add("e.name ILIKE @search");
            parameters.Add("search", $"%{query.Search}%", DbType.String);
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            conditions.Add("e.status = @status");
            parameters.Add("status", query.Status, DbType.String);
        }

        if (!string.IsNullOrWhiteSpace(query.EventType))
        {
            conditions.Add("e.type = @type");
            parameters.Add("type", query.EventType, DbType.String);
        }

        if (query.DateFrom.HasValue)
        {
            conditions.Add("e.event_date >= @date_from");
            parameters.Add("date_from", query.DateFrom?.ToUniversalTime(), DbType.DateTime);
        }

        if (query.DateTo.HasValue)
        {
            conditions.Add("e.event_date <= @date_to");
            parameters.Add("date_to", query.DateTo?.ToUniversalTime(), DbType.DateTime);
        }

        if (query.VenueId.HasValue)
        {
            conditions.Add("e.venue_id = @venue_id");
            parameters.Add("venue_id", query.VenueId, DbType.Guid);
        }


        parameters.Add("offset", (query.Pagination.Page - 1) * query.Pagination.PageSize, DbType.Int32);
        parameters.Add("pageSize", query.Pagination.PageSize, DbType.Int32);

        if (query.MinAvailableSeats.HasValue)
        {
            conditions.Add(
                """
                   ((SELECT COUNT(*) FROM seats s WHERE s.venue_id = e.venue_id) - 
                COALESCE((SELECT COUNT(*)
                          FROM reservation_seats rs
                                   JOIN reservations r ON rs.reservation_id = r.id
                          WHERE rs.event_id = e.id
                            AND r.status IN ('Confirmed', 'Pending')), 0)) >= @min_available_seats
                """);
            parameters.Add("min_available_seats", query.MinAvailableSeats.Value, DbType.Int32);
        }

        // Строим WHERE clause
        var whereClause = conditions.Count > 0
            ? "WHERE " + string.Join(" AND ", conditions)
            : string.Empty;

        long? totalCount = null;

        var sql = $"""
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
                   (SELECT COUNT(*) FROM seats
                     WHERE s.venue_id = e.venue_id) as total_seats,
                     
                   ( SELECT COUNT(*) FROM reservation_seats rs
                     JOIN reservations r ON rs.reservation_id = r.id
                     WHERE rs.events_id = e.id
                     WHERE r.status IN ('Confirmed', 'Pending') ) as reserved_seats,
                    () as available_seats,

                   ( (SELECT COUNT(*) FROM seats
                   WHERE s.venue_id = e.venue_id) -
                   ( SELECT COUNT(*) FROM reservation_seats rs
                   JOIN reservations r ON rs.reservation_id = r.id
                   WHERE rs.events_id = e.id
                   WHERE r.status IN ('Confirmed', 'Pending') ) ) as available_seats,

                   COUNT(*) OVER() AS total_count
                   FROM events e
                   JOIN events_details ON e.id = ed.event_id
                   -- JOIN seats s ON s.venue_id = e.venue_id
                   {whereClause}
                   ORDER BY e.event_date DESC
                   LIMIT @pageSize OFFSET @offset
                   """;

        var events = await connection
            .QueryAsync<EventDto, long, EventDto>(
                sql: sql,
                splitOn: "total_count",
                map: (@event, count) =>
                {
                    totalCount ??= count;

                    return @event;
                },
                param: parameters);

        return new GetEventsDto(events.ToList(), totalCount ?? 0);
    }
}