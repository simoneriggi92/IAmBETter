namespace iambetter.Domain.Entities.Database.DTO
{
    public class PredicitonHistoryDTO : PredictionDTO
    {
        public PredictionStatus PredictionStatus { get; set; }

        public string ActualResult { get; set; } = string.Empty;

        public string? FinalResult
        {
            get
            {
                return ActualResult switch
                {
                    "1" => "1",
                    "0" => "X",
                    "-1" => "2",
                    _ => null
                };
            }
        }
    }

    public enum PredictionStatus
    {
        Success = 0,
        Failed = 1,
    }
}
