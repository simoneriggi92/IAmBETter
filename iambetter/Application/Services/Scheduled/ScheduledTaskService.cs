using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iambetter.Application.Services.Interfaces;

namespace iambetter.Application.Services.Scheduled
{
    public class ScheduledTaskService : BackgroundService
    {
        public readonly ILogger<ScheduledTaskService> _logger;
        public readonly IServiceScopeFactory _serviceScopeFactory;
        private static readonly TimeSpan _interval = TimeSpan.FromMinutes(1); //Define the interval

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
            var taskManager = scope.ServiceProvider.GetRequiredService<IScheduledTaskManager>();

            _logger.LogInformation("Processing scheduled tasks.");

            try
            {
                await taskManager.RunPendingTasksAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing scheduled tasks.");
            }
        }
    }
}