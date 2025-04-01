using iambetter.Domain.Entities.Models;
using System.Text.Json.Serialization;

namespace iambetter.Domain.Entities.API
{
    public class APIStatsResponse
    {
        public string Get { get; set; }
        public Dictionary<string, string>? Parameters { get; set; }

        [JsonIgnore]
        public List<string> Errors { get; set; }
        public int Results { get; set; }
        public Paging Paging { get; set; }
        public TeamStatisticsResponse Response { get; set; }
    }

    public class Paging
    {
        public int Current { get; set; }
        public int Total { get; set; }
    }

    public class TeamStatisticsResponse
    {
        public LeagueInfo League { get; set; }
        public Team Team { get; set; }
        public string Form { get; set; }
        public Fixtures Fixtures { get; set; }
        public Goals Goals { get; set; }
        public Biggest Biggest { get; set; }
        public CleanSheet CleanSheet { get; set; }
        public FailedToScore FailedToScore { get; set; }
        public Penalty Penalty { get; set; }
        public List<Lineup> Lineups { get; set; }
        public Cards Cards { get; set; }
    }

    public class Fixtures
    {
        public FixtureStats Played { get; set; }
        public FixtureStats Wins { get; set; }
        public FixtureStats Draws { get; set; }
        public FixtureStats Loses { get; set; }
    }

    public class FixtureStats
    {
        public int Home { get; set; }
        public int Away { get; set; }
        public int Total { get; set; }
    }

    public class Goals
    {
        public GoalStats For { get; set; }
        public GoalStats Against { get; set; }
    }

    public class GoalStats
    {
        public FixtureStats Total { get; set; }
        public GoalAverage Average { get; set; }
        public Dictionary<string, GoalMinute> Minute { get; set; }
        public Dictionary<string, UnderOver> UnderOver { get; set; }
    }

    public class GoalAverage
    {
        public string Home { get; set; }
        public string Away { get; set; }
        public string Total { get; set; }
    }

    public class GoalMinute
    {
        public int? Total { get; set; }
        public string Percentage { get; set; }
    }

    public class UnderOver
    {
        public int Over { get; set; }
        public int Under { get; set; }
    }

    public class Biggest
    {
        public Streak Streak { get; set; }
        public Dictionary<string, string> Wins { get; set; }
        public Dictionary<string, string> Loses { get; set; }
        public BiggestGoals Goals { get; set; }
    }

    public class Streak
    {
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Loses { get; set; }
    }

    public class BiggestGoals
    {
        public Dictionary<string, int> For { get; set; }
        public Dictionary<string, int> Against { get; set; }
    }

    public class CleanSheet
    {
        public int Home { get; set; }
        public int Away { get; set; }
        public int Total { get; set; }
    }

    public class FailedToScore
    {
        public int Home { get; set; }
        public int Away { get; set; }
        public int Total { get; set; }
    }

    public class Penalty
    {
        public PenaltyStats Scored { get; set; }
        public PenaltyStats Missed { get; set; }
        public int Total { get; set; }
    }

    public class PenaltyStats
    {
        public int Total { get; set; }
        public string Percentage { get; set; }
    }

    public class Lineup
    {
        public string Formation { get; set; }
        public int Played { get; set; }
    }

    public class Cards
    {
        public Dictionary<string, CardStats> Yellow { get; set; }
        public Dictionary<string, CardStats> Red { get; set; }
    }

    public class CardStats
    {
        public int? Total { get; set; }
        public string Percentage { get; set; }
    }
}
