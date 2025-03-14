using iambetter.Domain.Entities.Database.Projections;
using iambetter.Domain.Entities.Models;

namespace iambetter.Domain.Entities.Database
{
    public class Season
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public List<TeamStatsProjection> TeamStatistics { get; set; } = new List<TeamStatsProjection>();

        /// <summary>
        /// The league may have multiple seasons, so multiple league info per season
        /// </summary>
        public LeagueInfo LeagueInfo { get; set; }
    }
}
