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
        private readonly BaseDataService<MatchDTO> _statsDataService;
        private readonly IAIDataSetService _dataSetComposerService;

        public IndexModel(ILogger<IndexModel> logger, APIService apiService, BaseDataService<Team> teamRepoService, BaseDataService<MatchDTO> statsDataService, IAIDataSetService dataSetComposerService)
        {
            _logger = logger;
            _apiService = apiService;
            _teamRepoService = teamRepoService;
            _statsDataService = statsDataService;
            _dataSetComposerService = dataSetComposerService;
        }

        public async Task OnGet()
        {
            //var result = await _dataSetService.GetTeamsAsync(2024);
            //await (_teamRepoService as TeamDataService).AddTeamsAsync(result, 135);
            //var result = await _dataSetService.GetNextRoundMatches(2024, 10);
            //await (_matchDataService as MatchDataService).SveNextMatchesAsync(result);
            //Domain.Entities.API.TeamStatisticsResponse? response = await _dataSetService.GetTeamStatisticsAsync(496, 2024);
            //var result = await (_statsDataService as StatsDataService).UpsertTeamStatsAsync(response);
            // var teamIds = await (_teamRepoService as TeamDataService).GetAllTeamsIdsBySeasonAndLeagueAsync(135, 2024);

            //var stats = await _dataSetService.GetAllTeamsStatisticsAsync(teamIds, 2024);

            // var r = await _dataSetService.GetAllTeamsStatisticsAsync(teamIds, 2024);
            // await (_statsDataService as StatsDataService).UpsertAllTeamsStatsAsync(r);
            // await (_statsDataService as StatsDataService).AddNextMatchesStatsGroupByRoundAsync(_apiDataService, (_teamRepoService as TeamDataService), 2024, 135);
            _logger.LogInformation("Runtime type of _matchDataService: {Type}", _statsDataService.GetType());
            var service = _statsDataService as StatsDataService;
             await service.FillLastRoundStatistics(_apiService);
        }
    }
}
