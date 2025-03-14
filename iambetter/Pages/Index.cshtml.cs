using iambetter.Application.Services.API;
using iambetter.Application.Services.Database;
using iambetter.Application.Services.Database.Abstracts;
using iambetter.Domain.Entities.Database.Projections;
using iambetter.Domain.Entities.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace iambetter.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly APIDataSetService _dataSetService;
        private readonly BaseDataService<Team> _teamRepoService;
        private readonly BaseDataService<MatchProjection> _matchDataService;
        private readonly BaseDataService<TeamStatsProjection> _statsDataService;

        public IndexModel(ILogger<IndexModel> logger, APIDataSetService dataSetService, BaseDataService<Team> teamRepoService, BaseDataService<MatchProjection> matchDataService, BaseDataService<TeamStatsProjection> statsDataService)
        {
            _logger = logger;
            _dataSetService = dataSetService;
            _teamRepoService = teamRepoService;
            _matchDataService = matchDataService;
            _statsDataService = statsDataService;
        }

        public async Task OnGet()
        {
            //var result = await _dataSetService.GetTeamsAsync(2024);
            //await (_teamRepoService as TeamDataService).AddTeamsAsync(result);
            //var result = await _dataSetService.GetNextRoundMatches(2024, 10);
            //await (_matchDataService as MatchDataService).SveNextMatchesAsync(result);
            Domain.Entities.API.TeamStatisticsResponse? response = await _dataSetService.GetTeamStatisticsAsync(496, 2024);
            var result = await (_statsDataService as StatsDataService).UpsertTeamStatsAsync(response);
        }
    }
}
