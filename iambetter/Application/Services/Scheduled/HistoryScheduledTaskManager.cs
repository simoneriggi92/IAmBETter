
using iambetter.Application.Services.Database;
using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.Database.DTO;

namespace iambetter.Application.Services.Scheduled
{
    public class HistoryScheduledTaskManager : BaseScheduledTaskManager
    {
        private readonly ILogger<ScheduledTaskManager> _logger;
        private readonly ITaskRepository _taskRepository;
        private readonly PredictionHistoryService _predictionHistoryService;

        public HistoryScheduledTaskManager(ILogger<ScheduledTaskManager> logger, ITaskRepository taskRepository, BaseDataService<PredicitonHistoryDTO> predictionHistoryService)
        {
            _logger = logger;
            _taskRepository = taskRepository;
            _predictionHistoryService = predictionHistoryService as PredictionHistoryService;
        }

        public override async Task RunPendingTasksAsync()
        {
            var lastExecutionTime = await _taskRepository.GetLastExecutionTimeAsync("MyHistoryScheduledTask");

            if (ShouldRunTask(lastExecutionTime)) // Define the interval
            {
                try
                {
                    // Run your scheduled tasks here
                    _logger.LogInformation("Running scheduled task...");

                    await ExecuteTaskAsync();

                    // Update the last execution time
                    await _taskRepository.UpdateLastExecutionTimeAsync("MyHistoryScheduledTask", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, " Error running scheduled tasks.");
                }
            }
            else
            {
                _logger.LogInformation("Scheduled tasks are not due to run yet.");
            }
        }
        public override async Task ExecuteTaskAsync()
        {
            await _predictionHistoryService.MovePredictionsToHistoryAsync();

            _logger.LogInformation("Predictions moved to history successfully.");
        }
    }
}
