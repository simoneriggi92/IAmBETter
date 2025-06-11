using iambetter.Application.Services.Interfaces;

namespace iambetter.Application.Services.Scheduled
{
    public abstract class BaseScheduledTaskManager : IScheduledTaskManager
    {
        public abstract Task RunPendingTasksAsync();
        public abstract Task ExecuteTaskAsync();
        public virtual bool ShouldRunTask(DateTime lastExecutionTime)
        {
            return (DateTime.UtcNow - lastExecutionTime).TotalHours >= 24; // Example:
        }
    }
}
