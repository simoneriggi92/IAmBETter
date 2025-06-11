using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iambetter.Application.Services.Database.Interfaces
{
    public interface ITaskRepository
    {
        Task<DateTime>? GetLastExecutionTimeAsync(string taskName);
        Task UpdateLastExecutionTimeAsync(string taskName, DateTime executionTime);
    }
}