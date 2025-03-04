using iambetter.Application.Services;
using iambetter.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace iambetter.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly PredictionService _predictionSerice;
        private readonly DataSetService _dataSetService;

        [BindProperty]
        public Bet? Bet { get; set; }

        public IndexModel(ILogger<IndexModel> logger, PredictionService predictionService, DataSetService dataSetService)
        {
            _logger = logger;
            _predictionSerice = predictionService;
            _dataSetService = dataSetService;
            Bet = new Bet();
        }

        public async Task OnGet()
        {
            await _dataSetService.GetUpcomingSerieAMatchesAsync(2024);
            Bet.Predictions = _predictionSerice.GetPredictions();
        }
    }
}
