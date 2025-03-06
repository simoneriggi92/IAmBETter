using static iambetter.Domain.Entities.Bet;

namespace iambetter.Application.Services
{
    public class PredictionService
    {
        public List<Prediction> GetPredictions()
        {
            return new List<Prediction>
            {
                new Prediction
                {
                    HomeTeam = "Liverpool",
                    AwayTeam = "Manchester United",
                    PredictedOutcome = "Home Win",
                    Confidence = 0.8m
                },
                new Prediction
                {
                    HomeTeam = "Manchester City",
                    AwayTeam = "Chelsea",
                    PredictedOutcome = "Draw",
                    Confidence = 0.6m
                },
                new Prediction
                {
                    HomeTeam = "Arsenal",
                    AwayTeam = "Tottenham",
                    PredictedOutcome = "Away Win",
                    Confidence = 0.7m
                }
            };
        }
    }
}
