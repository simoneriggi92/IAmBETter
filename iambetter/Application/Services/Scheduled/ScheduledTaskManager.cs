using iambetter.Application.Services.API;
using iambetter.Application.Services.Database;
using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Application.Services.Interfaces;
using iambetter.Domain.Entities.Database.Projections;
using iambetter.Domain.Entities.Models;

namespace iambetter.Application.Services.Scheduled
{
    public class ScheduledTaskManager : IScheduledTaskManager
    {
        private readonly ILogger<ScheduledTaskManager> _logger;
        private readonly ITaskRepository _taskRepository;
        private readonly MatchDataService _matchDataService;
        private readonly IAIDataSetService _dataSetComposerService;
        private readonly APIService _apiService;
        private readonly TeamDataService _teamRepoService;
        private const string SERIEA_LEAGUE_ID = "135";

        public ScheduledTaskManager(ILogger<ScheduledTaskManager> logger, ITaskRepository taskRepository, BaseDataService<MatchDTO> matchDataService, IAIDataSetService dataSetComposerService, APIService apiService, BaseDataService<Team> teamRepoService)
        {
            _logger = logger;
            _taskRepository = taskRepository;
            _matchDataService = matchDataService as MatchDataService;
            _dataSetComposerService = dataSetComposerService;
            _apiService = apiService;
            _teamRepoService = teamRepoService as TeamDataService;
        }
        
        public async Task RunPendingTasksAsync()
        {
            var lastExecutionTime = await _taskRepository.GetLastExecutionTimeAsync("MyScheduledTask");

            if(ShouldRunTask(lastExecutionTime)) // Define the interval
            {
                try
                {
                    // Run your scheduled tasks here
                    _logger.LogInformation("Running scheduled tasks...");

                    await ExecuteTaskAsync();

                    // Update the last execution time
                    await _taskRepository.UpdateLastExecutionTimeAsync("MyScheduledTask", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error running scheduled tasks.");
                }
            }
            else
            {
                _logger.LogInformation("Scheduled tasks are not due to run yet.");
            }
        }

        public bool ShouldRunTask(DateTime lastExecutionTime)
        {
                return (DateTime.UtcNow - lastExecutionTime).TotalHours >= 24; // Example:
        }

        private async Task ExecuteTaskAsync()
        {
            await _matchDataService.GetNextRoundMatchesWithStatsAsync(_apiService, _teamRepoService, 135);
            // Perform the actual scheduled task (e.g., data processing, cleanup, etc.)
            // await Task.Delay(1000); // Simulate work
            _logger.LogInformation("Scheduled task completed successfully.");
        }
    }
}