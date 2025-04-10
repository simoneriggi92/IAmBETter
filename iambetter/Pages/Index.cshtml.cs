using iambetter.Application.Services.API;
using iambetter.Application.Services.Database;
using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Interfaces;
using iambetter.Domain.Entities.Database.Projections;
using iambetter.Domain.Entities.Models;
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

        public IndexModel(ILogger<IndexModel> logger, APIService apiService, BaseDataService<Team> teamRepoService, BaseDataService<MatchDTO> matchDataService, IAIDataSetService dataSetComposerService)
        {
            _logger = logger;
            _apiService = apiService;
            _teamRepoService = teamRepoService;
            _matchDataService = matchDataService;
            _dataSetComposerService = dataSetComposerService;
        }

        public async Task OnGet()
        {
            var service = _matchDataService as MatchDataService;
            //  await service.FillLastRoundStatistics(_apiService);
            var matches = await service.GetAllMatchesAsync();
            _dataSetComposerService.GenerateDataSet(matches);
        }
    }
}
