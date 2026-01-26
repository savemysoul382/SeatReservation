// SeatReservation.Domain

using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Shared;

namespace SeatReservation.Domain.Venues;

public record VenueName
{
    public VenueName(string name, string prefix)
    {
        Name = name;
        Prefix = prefix;
    }

    // EF Core ctor
    [UsedImplicitly]
    private VenueName()
    {
    }

    public string Name { get; }

    public string Prefix { get; }

    public override string ToString() => $"{Prefix}-{Name}";

    public static Result<VenueName, Error> CreateWithoutPrefix(string name)
    {
        if (string.IsNullOrWhiteSpace(value: name))
        {
            return Error.Validation("venue.name", "Name is required");
        }

        if (name.Length > LengthConstants.LENGTH500)
        {
            return Error.Validation("venue.name", "Venue name is too long");
        }

        return new VenueName(name: name, prefix: string.Empty);
    }

    public static Result<VenueName, Error> Create(string name, string prefix)
    {
        if (string.IsNullOrWhiteSpace(value: name) || string.IsNullOrWhiteSpace(value: prefix))
        {
            return Error.Validation("venue.name", "Name and prefix are required");
        }

        if (prefix.Length > LengthConstants.LENGTH50 || name.Length > LengthConstants.LENGTH500)
        {
            return Error.Validation("venue.name", "Venue name is too long");
        }

        return new VenueName(name: name, prefix: prefix);
    }
}