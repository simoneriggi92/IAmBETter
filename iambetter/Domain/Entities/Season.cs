using iambetter.Domain.Models;

namespace iambetter.Domain.Entities
{
    public class Season
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public List<Team> PartecipantTeams { get; set; } = new List<Team>();

        /// <summary>
        /// The league may have multiple seasons, so multiple league info per season
        /// </summary>
        public LeagueInfo LeagueInfo { get; set; }
    }
}
