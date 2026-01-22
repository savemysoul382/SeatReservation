namespace SeatReservation.Domain
{
    public class User
    {
        public User()
        {
        }

        public Guid Id { get; set; }

        public Details Details { get; set; }
    }
}

public record Details
{
    public Details()
    {
    }

    public IReadOnlyList<SocialNetwork> Socials { get; set; }

    public string FIO { get; set; }

    public string Description { get; set; }
}

public record SocialNetwork
{
    public SocialNetwork()
    {
    }

    public string Name { get; init; }
    public string Link { get; init; }
}