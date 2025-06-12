using iambetter.Application.Services.Database;
using iambetter.Application.Services.Database.Abstracts;
using iambetter.Domain.Entities.Database.DTO;
using iambetter.Domain.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace iambetter.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly PredictionService _predictionService;
        private readonly LeagueInfoService _leagueInfoService;
        private readonly PredictionHistoryService? _predictionHistoryService;

        [BindProperty]
        public UserInput Input { get; set; }

        public IndexModel(ILogger<IndexModel> logger,
            BaseDataService<PredictionDTO> predictionService,
            BaseDataService<LeagueInfoDTO> leagueInfoService,
            BaseDataService<PredicitonHistoryDTO> predictionHistoryService
            )
        {
            _logger = logger;
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
                    MaxRounds = leagueInfo.MaxRounds
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
            var simplified = _history
                .OrderBy(p => p.MatchDate)
                .OrderByDescending(p => Convert.ToInt32(p.Round))
                .Select(p => new
                {
                    round = p.Round,
                    matchDate = $"{p.MatchDate.ToString("yyyy-MM-dd")} UTC",
                    matchTime = $"{p.MatchDate.ToString("hh:mm")} UTC",
                    teamA = p.HomeTeam.Name,
                    teamB = p.AwayTeam.Name,
                    predictedResult = p.PredictedResult,
                    finalResult = p.FinalResult,
                    status = p.PredictionStatus.ToString()
                });

            return new JsonResult(simplified);
        }

        public class UserInput
        {
            public bool IsLeagueTerminated => this.League.Round == this.League.MaxRounds;
            public LeagueInfo League { get; set; }
            public IEnumerable<PredictionDTO> Predictions { get; set; } = new List<PredictionDTO>();
            public IEnumerable<PredicitonHistoryDTO> PredicitonHistories { get; set; } = new List<PredicitonHistoryDTO>();
        }
    }
}
