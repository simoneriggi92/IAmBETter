namespace iambetter.Domain.Entities
{
    public class Bet
    {
        public int Id { get; set; }

        public List<Prediction> Predictions = new List<Prediction>();

        public class Prediction
        {
            public string HomeTeam { get; set; }

            public string AwayTeam { get; set; }

            public string PredictedOutcome { get; set; }

            public decimal Confidence { get; set; }
        }
    }
}
