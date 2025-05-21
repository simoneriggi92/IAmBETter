namespace iambetter.Domain.Entities.Database.DTO
{
    public class PredicitonHistoryDTO : PredictionDTO
    {
        public PredictionStatus PredictionStatus { get; set; }

        public string ActualResult { get; set; } = string.Empty;
    }

    public enum PredictionStatus
    {
        Success = 0,
        Failed = 1,
    }
}
