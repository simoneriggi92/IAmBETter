using iambetter.Application.Services.Interfaces;

namespace iambetter.Application.Services.Scheduled
{
    public class ScheduledTaskService : BackgroundService
    {
        public readonly ILogger<ScheduledTaskService> _logger;
        public readonly IServiceScopeFactory _serviceScopeFactory;
        private static readonly TimeSpan _interval = TimeSpan.FromMinutes(2); //Define the interval

        public ScheduledTaskService(ILogger<ScheduledTaskService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduled task service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessScheduledTasksAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing scheduled tasks.");
                }

                // Wait for the next interval
                Task.Delay(_interval, stoppingToken).Wait(stoppingToken);
            }
        }

        private async Task ProcessScheduledTasksAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var predictionTaskManager = scope.ServiceProvider.GetRequiredService<ScheduledTaskManager>();
            var predictionHistoryTaskManager = scope.ServiceProvider.GetRequiredService<HistoryScheduledTaskManager>();

            var taskManagers = new List<IScheduledTaskManager> { predictionTaskManager, predictionHistoryTaskManager };

            _logger.LogInformation("Processing scheduled tasks.");

            try
            {

                //run all scheduled tasks in parallel
                var tasks = taskManagers.Select(manager => manager.RunPendingTasksAsync()).ToArray();
                await Task.WhenAll(tasks);

                _logger.LogInformation("Scheduled tasks processed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing scheduled tasks.");
            }
        }
    }
}