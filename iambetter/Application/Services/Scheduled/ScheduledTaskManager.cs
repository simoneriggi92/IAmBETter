using iambetter.Application.Services.Database.Interfaces;
using iambetter.Application.Services.Interfaces;

namespace iambetter.Application.Services.Scheduled
{
    public class ScheduledTaskManager : IScheduledTaskManager
    {
        private readonly ILogger<ScheduledTaskManager> _logger;
        private readonly ITaskRepository _taskRepository;

        public ScheduledTaskManager(ILogger<ScheduledTaskManager> logger, ITaskRepository taskRepository)
        {
            _logger = logger;
            _taskRepository = taskRepository;
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
            // Perform the actual scheduled task (e.g., data processing, cleanup, etc.)
            await Task.Delay(1000); // Simulate work
            _logger.LogInformation("Scheduled task completed successfully.");
        }
    }
}