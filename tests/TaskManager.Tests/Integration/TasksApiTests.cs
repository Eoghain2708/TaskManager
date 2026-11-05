using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TaskManager.Tests.Integration
{
    public class TasksApiTests
    {

        [Fact]
        public async Task GetTasks_ShouldReturnSuccessAndJson()
        {
            using var factory = new CustomWebApplicationFactory();
            factory.InitialiseDatabase();
            using var client = factory.CreateClient();
            var response = await client.GetAsync("/api/tasks");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task GetTasks_ShouldReturnEmptyListForNewDatabase()
        {
            using var factory = new CustomWebApplicationFactory();
            factory.InitialiseDatabase();
            using var client = factory.CreateClient();
            var response = await client.GetAsync("/api/tasks");
            var tasks = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal(JsonValueKind.Array, tasks.ValueKind);
            Assert.Equal(0, tasks.GetArrayLength());
        }

        [Fact]
        public async Task CreateTask_ShouldReturnTaskWhenValid()
        {
            using var factory = new CustomWebApplicationFactory();
            factory.InitialiseDatabase();
            using var client = factory.CreateClient();
            var request = new
            {
                Title = "Integration test",
                Description = "Testing for creation"
            };

            var response = await client.PostAsJsonAsync("/api/tasks", request);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdTask = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal("Integration test", createdTask.GetProperty("title").GetString());

            Assert.True(createdTask.TryGetProperty("id", out _));
        }

        [Fact]
        public async Task CreateTask_ReturnsBadRequestWhenInvalid()
        {
            using var factory = new CustomWebApplicationFactory();
            factory.InitialiseDatabase();
            using var client = factory.CreateClient();
            var request = new
            {
                Description = "Description only"
            };
            var response = await client.PostAsJsonAsync("/api/tasks", request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }

        [Fact]
        public async Task GetTaskById_ReturnsNotFoundWhenTaskNotExist()
        {
            using var factory = new CustomWebApplicationFactory();
            factory.InitialiseDatabase();
            using var client = factory.CreateClient();
            var id = Guid.NewGuid();
            var response = await client.GetAsync($"/api/tasks/{id}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}