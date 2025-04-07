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

        public void GenerateDataSet(IEnumerable<TeamStatsDTO> stats)
        {
            try
            {
                var records = new List<TeamStatsRecord>();
                foreach (var stat in stats)
                {
                    var record = new TeamStatsRecord()
                    {
                        TeamId = Convert.ToInt16(stat.TeamStatistics.Team.Id),
                        Played = stat.TeamStatistics.Fixtures.Played.Total,
                        Wins = stat.TeamStatistics.Fixtures.Wins.Total,
                        Loses = stat.TeamStatistics.Fixtures.Loses.Total,
                        GoalsFor = stat.TeamStatistics.Goals.For.Total.Total,
                        GoalsAgainst = stat.TeamStatistics.Goals.Against.Total.Total,
                        CleanSheets = stat.TeamStatistics.CleanSheet?.Total ?? 0,
                        FailedToScore = stat.TeamStatistics.FailedToScore?.Total ?? 0,
                        PenaltiesScored = stat.TeamStatistics.Penalty.Scored.Total,
                        PenaltiesMissed = stat.TeamStatistics.Penalty.Missed.Total,
                        StreakWins = stat.TeamStatistics.Biggest.Streak.Wins,
                        StreakLoses = stat.TeamStatistics.Biggest.Streak.Loses,
                        YellowCardsTotal = stat.TeamStatistics.Cards.Yellow.Values.Sum(y => y.Total ?? 0),
                        RedCardsTotal = stat.TeamStatistics.Cards.Red.Values.Sum(r => r.Total ?? 0)
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

        public void WriteCsv<T>(string filePath, List<T> records)
        {
            if (records == null || records.Count == 0)
                return;

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Write header row (column names)
                writer.WriteLine(string.Join(",", properties.Select(p => p.Name)));

                // Write data rows
                foreach (var record in records)
                {
                    writer.WriteLine(string.Join(",", properties.Select(p => p.GetValue(record, null))));
                }
            }
        }
    }
}
