namespace iambetter.Domain.Models
{
    public class TeamResponse
    {
        public Team Team { get; set; }
        public Venue Venue { get; set; }
    }

    public class Team : TeamDetails
    {
        public string Code { get; set; }

        public string Country { get; set; }

        public int? Founded { get; set; }

        public bool National { get; set; }
    }

    public class Venue : VenueInfo
    {

        public string Surface { get; set; }

        public int? Capacity { get; set; }
    }
}
