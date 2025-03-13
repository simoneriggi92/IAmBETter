using iambetter.Application.Services;
using iambetter.Application.Services.Abstracts;
using iambetter.Domain.Entities;
using iambetter.Domain.Entities.Models;
using iambetter.Domain.Entities.Projections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace iambetter.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly APIDataSetService _dataSetService;
        private readonly BaseDataService<Team> _teamRepoService;
        private readonly BaseDataService<MatchProjection> _matchDataService;

        [BindProperty]
        public Bet? Bet { get; set; }

        public IndexModel(ILogger<IndexModel> logger, APIDataSetService dataSetService, BaseDataService<Team> teamRepoService, BaseDataService<MatchProjection> matchDataService)
        {
            _logger = logger;
            _dataSetService = dataSetService;
            _teamRepoService = teamRepoService;
            _matchDataService = matchDataService;
            Bet = new Bet();
        }

        public async Task OnGet()
        {
            //var result = await _dataSetService.GetTeamsAsync(2024);
            //await (_teamRepoService as TeamDataService).AddTeamsAsync(result);
            var result = await _dataSetService.GetNextRoundMatches(2024, 10);
            await (_matchDataService as MatchDataService).SveNextMatchesAsync(result);
        }
    }
}
