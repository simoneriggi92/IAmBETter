namespace iambetter.Domain.Models
{
    public class TeamResponse
    {
        public Team Team { get; set; }
        public Venue Venue { get; set; }
    }

    public class Team
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public int? Founded { get; set; }

        public string? Logo { get; set; }

        public bool National { get; set; }
    }

    public class Venue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }

        public string Surface { get; set; }

        public int? Capacity { get; set; }
    }
}
