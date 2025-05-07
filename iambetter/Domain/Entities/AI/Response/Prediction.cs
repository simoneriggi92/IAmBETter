using Newtonsoft.Json;

namespace iambetter.Domain.Entities.AI.Response
{
    public class PredictionResponse
    {
        [JsonProperty("records")]
        public List<Prediction> Predictions { get; set; }
    }

    public class Prediction
    {
        [JsonProperty("TeamA_TeamId")]
        public int HomeTeamId { get; set; }

        [JsonProperty("TeamB_TeamId")]
        public int TeamB_TeamId { get; set; }

        [JsonProperty("Predicted_Result")]
        public string PredictedResult { get; set; } = string.Empty;
    }
}

