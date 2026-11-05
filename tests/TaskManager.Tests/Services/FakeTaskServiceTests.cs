using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Core.Models;
using TaskManager.Core.Services;

namespace TaskManager.Tests.Services
{
    public class FakeTaskServiceTests
    {


        [Fact]
        public async Task CreateTask_ShouldReturnAndCreateTask()
        {
            var service = new FakeTaskService();
            var task = await service.CreateTaskAsync("Test", "Not empty");
            Assert.NotEqual(Guid.Empty, task.Id);
            Assert.Equal("Test", task.Title);
            Assert.Equal("Not empty", task.Description);
            Assert.False(task.Completed);

        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        public async Task CreateTask_ShouldThrowIfBlankTitle(string title)
        {
            var service = new FakeTaskService();
            await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateTaskAsync(title, null));
        }

        [Fact]
        public async Task GetAllTasks_ShouldReturnAllTasks()
        {
            var service = new FakeTaskService();
            await service.CreateTaskAsync("Learn docker", null);
            await service.CreateTaskAsync("Learn kubernetes", null);
            var results = await service.GetAllTasksAsync();
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public async Task GetTaskById_ShouldReturnTask()
        {
            var service = new FakeTaskService();
            var task = await service.CreateTaskAsync("Testing", "Passing");
            var returned = await service.GetTaskByIdAsync(task.Id);
            Assert.NotNull(returned);
            Assert.Equal(task.Id, returned.Id);
        }

        [Fact]
        public async Task GetTaskById_ShouldReturnNullWhenIdNotExist()
        {
            var service = new FakeTaskService();
            await service.CreateTaskAsync("Testing", "Failing");
            var returned = await service.GetTaskByIdAsync(Guid.NewGuid());
            Assert.Null(returned);
        }

        [Fact]
        public async Task UpdateTask_ShouldUpdateProperties()
        {
            var service = new FakeTaskService();
            var task = await service.CreateTaskAsync("Testing", "Test this");
            await service.UpdateTaskAsync(task.Id, "Changed", "Changed the task");
            Assert.Equal("Changed", task.Title);
            Assert.Equal("Changed the task", task.Description);
        }

        [Fact]
        public async Task CompleteTask_SetsCompletedTrue()
        {
            var service = new FakeTaskService();
            var task = await service.CreateTaskAsync("Test", "test");
            await service.CompleteTaskAsync(task.Id);
            Assert.True(task.Completed);
        }

        [Fact]
        public async Task DeleteTask_ShouldRemoveCorrectTaskWhenTrue()
        {
            var service = new FakeTaskService();
            var task = await service.CreateTaskAsync("Test", null);
            var extra = await service.CreateTaskAsync("Test2", null);
            await service.DeleteTaskAsync(task.Id);
            var returned = await service.GetAllTasksAsync();
            Assert.Single(returned);
            Assert.Contains(extra, returned);
            Assert.DoesNotContain(task, returned);
        }

        [Fact]
        public async Task DeleteTask_ShowThrowWhenMissingId()
        {
            var service = new FakeTaskService();
            var task = await service.CreateTaskAsync("Test", null);
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.DeleteTaskAsync(Guid.NewGuid()));
        }
    }
}