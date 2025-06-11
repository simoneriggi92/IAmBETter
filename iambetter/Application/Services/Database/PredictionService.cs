using iambetter.Application.Services.API;
using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.Database.DTO;
using iambetter.Domain.Entities.Database.Projections;
using MongoDB.Driver;

namespace iambetter.Application.Services.Database
{
    public class PredictionService : BaseDataService<PredictionDTO>
    {
        private readonly ILogger<MatchDataService> _logger;
        private readonly MatchDataService _matchDataService;
        private readonly APIService _apiService;
        private readonly LeagueInfoService _leagueInfoService;

        public PredictionService(IRepositoryService<PredictionDTO> repositoryService, ILogger<MatchDataService> logger, BaseDataService<MatchDTO> matchDataService, APIService apiService, BaseDataService<LeagueInfoDTO> leagueInfoService) : base(repositoryService)
        {
            _logger = logger;
            _matchDataService = matchDataService as MatchDataService;
            _apiService = apiService as APIService;
            _leagueInfoService = leagueInfoService as LeagueInfoService;
        }

        public async Task SavePredictionsAsync(IEnumerable<PredictionDTO> predictions)
        {
            await InsertManyAsync(predictions);
        }

        public async Task<IEnumerable<PredictionDTO>> GetPredictionsByRoundAsync()
        {
            var leagueInfo = await _leagueInfoService.GetLeagueInfoAsync();

            if (leagueInfo == null)
            {
                _logger.LogWarning($"{nameof(PredictionService)}:: no league info stored in the db");
                return Enumerable.Empty<PredictionDTO>();
            }

            if (string.IsNullOrWhiteSpace(leagueInfo.CurrentRound))
            {
                _logger.LogWarning($"{nameof(PredictionService)}:: no round info stored yet");
                return Enumerable.Empty<PredictionDTO>();
            }

            //TODO: to be retrieved from the db
            var currentSeason = leagueInfo.Season;
            var leagueId = leagueInfo.LeagueId;

            var round = leagueInfo.CurrentRound;

            //get predictions where round is the same
            var filter = Builders<PredictionDTO>.Filter.And(
                Builders<PredictionDTO>.Filter.Eq(m => m.Round, round)
            );

            var preditictions = await GetByFilterAsync(filter, null, null);

            if (!preditictions.Any())
                _logger.LogWarning($"{nameof(PredictionService)}:: no predictions have been found for the round={round}");

            return preditictions;
        }
    }

}
