using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iambetter.Application.Services.Interfaces
{
    public interface IScheduledTaskManager
    {
        Task RunPendingTasksAsync();
    }
}