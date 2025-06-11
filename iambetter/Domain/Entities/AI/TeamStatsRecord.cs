namespace iambetter.Domain.Entities.AI
{
    public class MatchRecord
    {
        public TeamDataSetStats Home { get; set; } = new TeamDataSetStats();
        public TeamDataSetStats Away { get; set; } = new TeamDataSetStats();
        public string Result { get; set; } = string.Empty;
    }

    public class TeamDataSetStats{
        public int TeamId { get; set; }
        public int Played { get; set; }= 0;
        public int Wins { get; set; }= 0;
        public int Loses { get; set; }= 0;
        public int GoalsFor { get; set; }= 0;
        public int GoalsAgainst { get; set; }= 0;
        public int CleanSheets { get; set; }= 0;
        public int FailedToScore { get; set; }= 0;
        public int PenaltiesScored { get; set; } = 0;
        public int PenaltiesMissed { get; set; } = 0;
        public int StreakWins { get; set; }= 0;
        public int StreakLoses { get; set; }= 0;
        public int YellowCardsTotal { get; set; }= 0;
        public int RedCardsTotal { get; set; }= 0;
    }
}
