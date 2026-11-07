using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Core.Models;
using TaskManager.Core.Services;

namespace TaskManager.Api.Services
{
    public class EfTaskService : ITaskService
    {

        private readonly AppDbContext _context;

        public EfTaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TaskItem> CompleteTaskAsync(Guid id)
        {
            var task = await GetTaskByIdOrThrow(id);
            task.Completed = true;
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem> CreateTaskAsync(string title, string? description)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("taskitem does not have a title", nameof(title));
            }

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = title.Trim(),
                Description = description,
                CreatedAt = DateTime.UtcNow,
                Completed = false
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteTaskAsync(Guid id)
        {
            var task = await GetTaskByIdAsync(id);
            if (task is null)
            {
                return false;
            }
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<TaskItem>> GetAllTasksAsync()
        {
            return await _context.Tasks.AsNoTracking().ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(Guid id)
        {
            return await _context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskItem> UpdateTaskAsync(Guid id, string title, string? description)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException("taskitem must have title", nameof(title));
            }
            var task = await GetTaskByIdOrThrow(id);
            task.Title = title.Trim();
            task.Description = description;
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem> ChangeDateAsync(Guid id, DateTime dateTime)
        {
            var task = await GetTaskByIdOrThrow(id);
            task.CreatedAt = dateTime;
            await _context.SaveChangesAsync();
            return task;
        }

        private async Task<TaskItem> GetTaskByIdOrThrow(Guid id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id) ?? throw new KeyNotFoundException($"Task of id {id} cannot be found");
            return task;
        }
    }
}