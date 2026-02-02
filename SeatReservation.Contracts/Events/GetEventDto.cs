// SeatReservation.Contracts

using SeatReservation.Contracts.Seats;

namespace SeatReservation.Contracts.Events;

public record GetEventsDto(List<EvetDto> Events, long TotalCount);

public record EvetDto
{
    public Guid Id { get; init; }

    // навигационное свойство
    public int Capacity { get; init; }

    public string Description { get; init; } = string.Empty;

    public DateTime? LastReservationUtc { get; init; }

    public Guid VenueId { get; init; }

    public string Name { get; init; } = string.Empty;

    public DateTime EventDate { get; init; }

    public DateTime StartDate { get; init; }

    public DateTime EndDate { get; init; }

    public string Type { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Info { get; init; } = string.Empty;

    public int TotalSeats { get; init; }

    public int ReservedSeats { get; init; }

    public int AvailableSeats { get; init; }
};

public record GetEventDto
{
    public Guid Id { get; init; }

    // навигационное свойство
    public int Capacity { get; init; }

    public string Description { get; init; } = string.Empty;

    public DateTime? LastReservationUtc { get; init; }

    public Guid VenueId { get; init; }

    public string Name { get; init; } = string.Empty;

    public DateTime EventDate { get; init; }

    public DateTime StartDate { get; init; }

    public DateTime EndDate { get; init; }

    public string Type { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Info { get; init; } = string.Empty;

    public List<AvailableSeatDto> Seats { get; init; } = [];

    public int TotalSeats { get; init; }

    public int ReservedSeats { get; init; }

    public int AvailableSeats { get; init; }
}