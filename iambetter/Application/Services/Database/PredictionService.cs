using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.Database.DTO;

namespace iambetter.Application.Services.Database
{
    public class PredictionService : BaseDataService<PredictionDTO>
    {
        private readonly ILogger<MatchDataService> _logger;

        public PredictionService(IRepositoryService<PredictionDTO> repositoryService, ILogger<MatchDataService> logger) : base(repositoryService)
        {
            _logger = logger;
        }

        public async Task SavePredictionsAsync(IEnumerable<PredictionDTO> predictions)
        {
            await InsertManyAsync(predictions);
        }
    }
}
