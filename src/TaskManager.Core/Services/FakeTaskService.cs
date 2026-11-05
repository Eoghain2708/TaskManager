using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services
{
    public class FakeTaskService : ITaskService
    {
        private readonly List<TaskItem> _tasks = new();


        public async Task<TaskItem> CreateTaskAsync(string title, string? description)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Task title cannot be empty");
            }
            var item = new TaskItem { Title = title.Trim(), Description = description };
            _tasks.Add(item);
            return item;
        }


        public async Task<IReadOnlyList<TaskItem>> GetAllTasksAsync()
        {
            return _tasks.AsReadOnly();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(Guid id)
        {
            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        public async Task<TaskItem> UpdateTaskAsync(Guid id, string title, string? description)
        {
            var task = GetTaskByIdOrThrow(id);

            task.Title = title;
            task.Description = description;
            return task;
        }

        public async Task<TaskItem> CompleteTaskAsync(Guid id)
        {
            var task = GetTaskByIdOrThrow(id);
            task.Completed = true;
            return task;
        }



        public async Task<bool> DeleteTaskAsync(Guid id)
        {
            var task = GetTaskByIdOrThrow(id);
            if (task is null)
            {
                return false;
            }
            _tasks.Remove(task);
            return true;
        }


        private TaskItem GetTaskByIdOrThrow(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task is null)
            {
                throw new KeyNotFoundException($"Task of id {id} cannot be found");
            }
            return task;
        }
    }
}