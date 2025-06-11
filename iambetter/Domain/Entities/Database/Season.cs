using iambetter.Domain.Entities.Database.Projections;
using iambetter.Domain.Entities.Models;

namespace iambetter.Domain.Entities.Database
{
    public class Season
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public List<TeamStatsDTO> TeamStatistics { get; set; } = new List<TeamStatsDTO>();

        /// <summary>
        /// The league may have multiple seasons, so multiple league info per season
        /// </summary>
        public LeagueInfo LeagueInfo { get; set; }
    }
}
