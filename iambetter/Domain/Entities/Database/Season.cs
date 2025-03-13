using iambetter.Domain.Entities.Database.Projections;
using iambetter.Domain.Entities.Models;

namespace iambetter.Domain.Entities.Database
{
    public class Season
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public List<TeamStasProjection> TeamStatistics { get; set; } = new List<TeamStasProjection>();

        /// <summary>
        /// The league may have multiple seasons, so multiple league info per season
        /// </summary>
        public LeagueInfo LeagueInfo { get; set; }
    }
}
