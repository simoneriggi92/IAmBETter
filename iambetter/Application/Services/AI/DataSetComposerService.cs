using iambetter.Application.Services.Interfaces;
using iambetter.Domain.Entities.AI;
using iambetter.Domain.Entities.Database.Projections;
using System.Reflection;
using System.Text;

namespace iambetter.Application.Services.AI
{
    public class DataSetComposerService : IAIDataSetService
    {
        private readonly string DATA_SET_FILENAME = "dataset.csv";
        public DataSetComposerService()
        {
        }

        public void GenerateDataSet(IEnumerable<MatchDTO> matches)
        {
            try
            {
                var records = new List<MatchRecord>();
                foreach (var stat in matches)
                {
                    var homeTeamStats = stat.TeamStatistics.FirstOrDefault(x => x.Team.TeamId == stat.Teams.Home.TeamId);
                    var awayTeamStats = stat.TeamStatistics.FirstOrDefault(x => x.Team.TeamId == stat.Teams.Away.TeamId);

                    if(homeTeamStats == null || awayTeamStats == null)
                        continue;

                    var record = new MatchRecord()
                    {
                        Home = new TeamDataSetStats()
                        {
                            TeamId = Convert.ToInt16(stat.Teams.Home.TeamId),
                            Played = homeTeamStats.Fixtures.Played.Total,
                            Wins = homeTeamStats.Fixtures.Wins.Total ,
                            Loses = homeTeamStats.Fixtures.Loses.Total ,
                            GoalsFor = homeTeamStats.Goals.For.Total.Total ,
                            GoalsAgainst = homeTeamStats.Goals.Against.Total.Total ,
                            CleanSheets = homeTeamStats.CleanSheet?.Total ?? 0,
                            FailedToScore = homeTeamStats.FailedToScore?.Total ?? 0,
                            PenaltiesScored = homeTeamStats.Penalty.Scored.Total,
                            PenaltiesMissed = homeTeamStats.Penalty.Missed.Total,
                            StreakWins = homeTeamStats.Biggest.Streak.Wins,
                            StreakLoses = homeTeamStats.Biggest.Streak.Loses ,
                            YellowCardsTotal = homeTeamStats.Cards.Yellow.Values.Sum(y => y.Total ?? 0),
                            RedCardsTotal = homeTeamStats.Cards.Red.Values.Sum(r => r.Total ?? 0)
                        },
                        Away = new TeamDataSetStats()
                        {
                            TeamId = Convert.ToInt16(stat.Teams.Away.TeamId),
                            Played = awayTeamStats.Fixtures.Played.Total,
                            Wins = awayTeamStats.Fixtures.Wins.Total ,
                            Loses = awayTeamStats.Fixtures.Loses.Total ,
                            GoalsFor = awayTeamStats.Goals.For.Total.Total ,
                            GoalsAgainst = awayTeamStats.Goals.Against.Total.Total ,
                            CleanSheets = awayTeamStats.CleanSheet?.Total ?? 0,
                            FailedToScore = awayTeamStats.FailedToScore?.Total ?? 0,
                            PenaltiesScored = awayTeamStats.Penalty.Scored.Total,
                            PenaltiesMissed = awayTeamStats.Penalty.Missed.Total,
                            StreakWins = awayTeamStats.Biggest.Streak.Wins,
                            StreakLoses = awayTeamStats.Biggest.Streak.Loses,
                            YellowCardsTotal = awayTeamStats.Cards.Yellow.Values.Sum(y => y.Total ?? 0),
                            RedCardsTotal = awayTeamStats.Cards.Red.Values.Sum(r => r.Total ?? 0)
                        },
                        Result = stat.Result,
                    };
                    records.Add(record);
                }

                WriteCsv(DATA_SET_FILENAME, records);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void WriteCsv(string filePath, List<MatchRecord> records)
        {
            var csv = new StringBuilder();

            // Dynamically generate headers for Team A and Team B
            var teamStatsProperties = typeof(TeamDataSetStats).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var headers = teamStatsProperties.Select(p => $"TeamA_{p.Name}")
                .Concat(teamStatsProperties.Select(p => $"TeamB_{p.Name}"))
                .Concat(new[] { "Match_Result" });
            csv.AppendLine(string.Join(",", headers));

            // Write each record
            foreach (var record in records)
            {
                var teamAValues = teamStatsProperties.Select(p => p.GetValue(record.Home)?.ToString() ?? "0");
                var teamBValues = teamStatsProperties.Select(p => p.GetValue(record.Away)?.ToString() ?? "0");
                var resultValue = string.IsNullOrWhiteSpace(record.Result?.ToString()) ? "0" : record.Result;

                var line = string.Join(",", teamAValues.Concat(teamBValues).Concat(new[] { resultValue }));
                csv.AppendLine(line);
            }

            File.WriteAllText(filePath, csv.ToString());
        }
    }
}
