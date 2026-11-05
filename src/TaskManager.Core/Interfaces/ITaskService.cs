using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services
{
    public interface ITaskService
    {
        public Task<TaskItem> CreateTaskAsync(string title, string? description);
        public Task<IReadOnlyList<TaskItem>> GetAllTasksAsync();
        public Task<TaskItem?> GetTaskByIdAsync(Guid id);

        Task<TaskItem> UpdateTaskAsync(Guid id, string title, string? description);
        Task<TaskItem> CompleteTaskAsync(Guid id);
        Task<bool> DeleteTaskAsync(Guid id);
    }
}