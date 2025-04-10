using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iambetter.Application.Services.Database.Abstracts;
using iambetter.Application.Services.Database.Interfaces;
using iambetter.Domain.Entities.Database.DTO;

namespace iambetter.Application.Services.Database
{
    public class TaskRepository : BaseDataService<TaskDTO>, ITaskRepository
    {
        private readonly IRepositoryService<TaskDTO> _repositoryService;
        public TaskRepository(IRepositoryService<TaskDTO> repositoryService) : base(repositoryService)
        {
            _repositoryService = repositoryService;
        }
        
        public async Task<DateTime>? GetLastExecutionTimeAsync(string taskName)
        {
            return await _repositoryService.GetAsync(taskName)
                .ContinueWith(task =>
                {
                    var taskDTO = task.Result;
                    return taskDTO?.LastExecutionTime ?? DateTime.MinValue;
                });
        }

        public async Task UpdateLastExecutionTimeAsync(string taskName, DateTime executionTime)
        {
            var taskExecutionTime = await GetLastExecutionTimeAsync(taskName);

            if (taskExecutionTime == DateTime.MinValue)
            {
                // If the task does not exist, create a new one
                var newTask = new TaskDTO
                {
                    TaskName = taskName,
                    LastExecutionTime = executionTime
                };

                await _repositoryService.InsertAsync(newTask);
            }
            else
            {
                // If the task already exists, update its last execution time
                var taskDTO = await _repositoryService.GetAsync(taskName);

                if (taskDTO != null)
                {
                    taskDTO.LastExecutionTime = executionTime;
                    // Update the task in the database
                    await _repositoryService.UpdateAsync(taskName,  taskDTO);
                }
            }
        }
    }
}