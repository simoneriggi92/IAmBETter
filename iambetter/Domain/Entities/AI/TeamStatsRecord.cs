namespace iambetter.Domain.Entities.AI
{
    public class TeamStatsRecord
    {
        public int TeamId { get; set; }
        public int Played { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int? CleanSheets { get; set; }
        public int? FailedToScore { get; set; }
        public int PenaltiesScored { get; set; }
        public int PenaltiesMissed { get; set; }
        public int StreakWins { get; set; }
        public int StreakLoses { get; set; }
        public int YellowCardsTotal { get; set; }
        public int RedCardsTotal { get; set; }
    }
}
