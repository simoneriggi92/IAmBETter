namespace iambetter.Domain.Models
{
    public class ApiResponse<T>
    {
        public List<T> Response { get; set; } = new();
    }

    public class FixtureResponse
    {
        public FixtureInfo Fixture { get; set; }
        public GoalsInfo Goals { get; set; }
        public LeagueInfo League { get; set; }
        public ScoreInfo Score { get; set; }
        public TeamInfo Teams { get; set; }
    }

    public class FixtureInfo
    {
        public int? Id { get; set; }
        public string Date { get; set; }
        public VenueInfo Venue { get; set; }
        public StatusInfo Status { get; set; }
        public PeriodsInfo Periods { get; set; }
        public string Referee { get; set; }

        public string Timezone { get; set; }
    }

    public class PeriodsInfo
    {
        public int? First { get; set; }
        public int? Second { get; set; }
    }

    public class VenueInfo
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
    }

    public class StatusInfo
    {
        public string Long { get; set; }
        public string Short { get; set; }
        public int? Elapsed { get; set; }

        public int? Extra { get; set; }
    }

    public class GoalsInfo
    {
        public int? Home { get; set; }
        public int? Away { get; set; }
    }

    public class LeagueInfo
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Logo { get; set; }

        public string Flag { get; set; }

        public int? Season { get; set; }

        public string Round { get; set; }

        public bool Standings { get; set; }
    }

    public class ScoreInfo
    {
        public TimeScore Fulltime { get; set; }

        public TimeScore Halftime { get; set; }

        public TimeScore ExtraTime { get; set; }

        public TimeScore Penalty { get; set; }
    }

    public class TimeScore
    {
        public int? Home { get; set; }
        public int? Away { get; set; }
    }

    public class TeamInfo
    {
        public TeamDetails Home { get; set; }
        public TeamDetails Away { get; set; }
    }

    public class TeamDetails
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }

        public bool? Winner { get; set; }
    }

}
