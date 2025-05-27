using iambetter.Application.Services.API;
using iambetter.Application.Services.Database;
using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Interfaces;
using iambetter.Domain.Entities.Database.DTO;
using iambetter.Domain.Entities.Database.Projections;
using iambetter.Domain.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace iambetter.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly APIService _apiService;
        private readonly BaseDataService<Team> _teamRepoService;
        private readonly BaseDataService<MatchDTO> _matchDataService;
        private readonly IAIDataSetService _dataSetComposerService;
        private readonly PredictionService _predictionService;
        private readonly LeagueInfoService _leagueInfoService;
        private readonly PredictionHistoryService? _predictionHistoryService;

        [BindProperty]
        public UserInput Input { get; set; }

        public IndexModel(ILogger<IndexModel> logger,
            APIService apiService,
            BaseDataService<Team> teamRepoService,
            BaseDataService<MatchDTO> matchDataService,
            IAIDataSetService dataSetComposerService,
            BaseDataService<PredictionDTO> predictionService,
            BaseDataService<LeagueInfoDTO> leagueInfoService,
            BaseDataService<PredicitonHistoryDTO> predictionHistoryService
            )
        {
            _logger = logger;
            _apiService = apiService;
            _teamRepoService = teamRepoService;
            _matchDataService = matchDataService;
            _dataSetComposerService = dataSetComposerService;
            _predictionService = predictionService as PredictionService;
            _leagueInfoService = leagueInfoService as LeagueInfoService;
            _predictionHistoryService = predictionHistoryService as PredictionHistoryService;
        }

        public async Task OnGet()
        {
            var leagueInfo = await _leagueInfoService.GetLeagueInfoAsync();

            Input = new UserInput
            {
                League = new LeagueInfo()
                {
                    Round = leagueInfo.CurrentRound,
                    Season = leagueInfo.Season,
                    Name = leagueInfo.Name,
                },
            };

        }

        public async Task<JsonResult> OnGetPredictionsAsync()
        {
            var _predictions = await _predictionService.GetPredictionsByRoundAsync();
            var simplified = _predictions.Select(p => new
            {
                teamA = p.HomeTeam.Name,
                teamB = p.AwayTeam.Name,
                predictedResult = p.PredictedResult,
                round = p.Round
            });
            return new JsonResult(simplified);
        }

        public async Task<JsonResult> OnGetPredictionsHistoryAsync()
        {
            var _history = await _predictionHistoryService.GetAllAsync();
            var simplified = _history.Select(p => new
            {
                round = p.Round,
                teamA = p.HomeTeam.Name,
                teamB = p.AwayTeam.Name,
                predictedResult = p.PredictedResult,
                actualResult = p.ActualResult,
                status = p.PredictionStatus.ToString()
            });

            return new JsonResult(simplified);
        }

        public class UserInput
        {
            public LeagueInfo League { get; set; }
            public IEnumerable<PredictionDTO> Predictions { get; set; } = new List<PredictionDTO>();
            public IEnumerable<PredicitonHistoryDTO> PredicitonHistories { get; set; } = new List<PredicitonHistoryDTO>();
        }
    }
}
