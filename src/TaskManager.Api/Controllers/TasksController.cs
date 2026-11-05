using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Services;
using TaskManager.Api.DTOs;
using TaskManager.Core.Models;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TaskResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EndpointSummary("Get a list of all tasks")]
        [EndpointDescription("Get a big list of allllll the tasks.")]
        public async Task<ActionResult<List<TaskResponse>>> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks.Select(MapToResponse).ToList());
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskResponse>> GetTaskById(Guid id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task is null)
            {
                return NotFound();
            }

            return Ok(MapToResponse(task));
        }

        [HttpPost]
        [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Create a task")]
        [EndpointDescription("Create a task with required title field and optional description field")]
        public async Task<ActionResult<TaskResponse>> CreateTask(CreateTaskRequest r)
        {
            var task = await _taskService.CreateTaskAsync(r.Title, r.Description);
            var response = MapToResponse(task);

            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, response);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskResponse>> UpdateTask(Guid id, UpdateTaskRequest r)
        {
            try
            {
                var task = await _taskService.UpdateTaskAsync(id, r.Title, r.Description);
                return Ok(MapToResponse(task));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpPatch("{id:guid}/complete")]
        [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskResponse>> Complete(Guid id)
        {
            try
            {
                var task = await _taskService.CompleteTaskAsync(id);
                return Ok(MapToResponse(task));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var deleted = await _taskService.DeleteTaskAsync(id);
            if (deleted is false)
            {
                return NotFound();
            }
            return NoContent();
        }


        private static TaskResponse MapToResponse(TaskItem task)
        {
            return new TaskResponse
            {
                Id = task.Id,
                Description = task.Description,
                Title = task.Title,
                Completed = task.Completed,
                CreatedAt = task.CreatedAt
            };
        }
    }
}