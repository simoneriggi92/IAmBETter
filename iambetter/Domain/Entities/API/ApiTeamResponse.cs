using System.Text.Json.Serialization;

namespace iambetter.Domain.Entities.Models
{
    public class ApiTeamResponse
    {
        [JsonIgnore]
        public int Season { get; set; }
        public TeamDetails Team { get; set; }
        public Venue Venue { get; set; }
    }

    public class TeamDetails : Team
    {
        public string Code { get; set; }

        public string Country { get; set; }

        public int? Founded { get; set; }

        public bool National { get; set; }

        public int Season { get; set; }
    }

    public class Venue : VenueInfo
    {
        public string Surface { get; set; }

        public int? Capacity { get; set; }
    }
}
