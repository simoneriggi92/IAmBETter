using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.Database.DTO;
using iambetter.Domain.Entities.Database.Projections;
using MongoDB.Driver;

namespace iambetter.Application.Services.Database
{
    public class PredictionHistoryService : BaseDataService<PredicitonHistoryDTO>
    {
        private readonly ILogger<PredictionHistoryService> _logger;
        private readonly PredictionService _predictionService;
        private readonly MatchDataService _matchDataService;

        public PredictionHistoryService(IRepositoryService<PredicitonHistoryDTO> repositoryService,
            BaseDataService<PredictionDTO> predictionService,
            BaseDataService<MatchDTO> matchDataService,
            ILogger<PredictionHistoryService> logger) : base(repositoryService)
        {
            _logger = logger;
            _predictionService = predictionService as PredictionService;
            _matchDataService = matchDataService as MatchDataService;
        }

        public async Task SavePredictionsAsync(IEnumerable<PredicitonHistoryDTO> predictions)
        {
            await InsertManyAsync(predictions);
        }

        /// <summary>
        /// Move predictions of the matches already played to the history collection. Then delete old predictions
        /// from the collection to keep only fresh data
        /// </summary>
        /// <returns></returns>
        public async Task MovePredictionsToHistoryAsync()
        {
            var predictionsToMove = new List<PredicitonHistoryDTO>();
            // get all predictions where the match date is before now
            var filter = Builders<PredictionDTO>.Filter.And(
                Builders<PredictionDTO>.Filter.Lt(m => m.MatchDate, DateTime.UtcNow)
            );

            var candiatedPredictionsToMove = await _predictionService.GetByFilterAsync(filter, null);

            if (!candiatedPredictionsToMove.Any())
                return;

            //get all the matches where the matchid is in the predictions to move
            var predictionsMatches = await _matchDataService.GetByFilterAsync(
                Builders<MatchDTO>.Filter.In(m => m.Id, candiatedPredictionsToMove.Select(p => p.MatchId)),
                null
            );

            //check the result of the matches and update the predictions accordingly
            foreach (var prediction in candiatedPredictionsToMove)
            {
                var match = predictionsMatches.FirstOrDefault(m => m.Id == prediction.MatchId);

                if (match != null)
                {
                    predictionsToMove.Add(new PredicitonHistoryDTO()
                    {

                        MatchId = prediction.MatchId,
                        HomeTeam = match.Teams.Home,
                        AwayTeam = match.Teams.Away,
                        PredictedResult = prediction.PredictedResult,
                        CreationDateUtc = prediction.CreationDateUtc,
                        MatchDate = match.MatchDate,
                        Round = match.Round,
                        PredictionStatus = prediction.AIModelPredictedResult == match.Result
                            ? PredictionStatus.Success
                            : PredictionStatus.Failed,
                        LastUpdateDateUtc = DateTime.UtcNow,
                        ActualResult = match.Result
                    });
                }
                else
                    _logger.LogWarning($"Match with ID {prediction.MatchId} not found for prediction {prediction.Id}");
            }

            await InsertManyAsync(predictionsToMove);

            //delete all moved predictions to history to keep only predictions to be played yet
            var matchIdsToDelete = predictionsMatches.Select(x => x.Id).ToList();
            filter = Builders<PredictionDTO>.Filter.In(y => y.MatchId, matchIdsToDelete);

            await _predictionService.DeleteManyAsync(filter);
        }
    }
}