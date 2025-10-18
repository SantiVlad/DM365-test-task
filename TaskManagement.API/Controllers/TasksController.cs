using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs;
using TaskManagement.API.Models;
using TaskManagement.API.Services;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTask(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            
            if (task == null)
            {
                return NotFound(new { message = $"Task with ID {id} not found" });
            }
            
            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdTask = await _taskService.CreateTaskAsync(createTaskDto);
            
            if (createdTask == null)
            {
                return BadRequest(new { message = "Failed to create task" });
            }
            
            return CreatedAtAction(nameof(GetTask), new { id = createdTask.TaskId }, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedTask = await _taskService.UpdateTaskAsync(id, updateTaskDto);
            
            if (updatedTask == null)
            {
                return NotFound(new { message = $"Task with ID {id} not found" });
            }
            
            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            
            if (!result)
            {
                return NotFound(new { message = $"Task with ID {id} not found" });
            }
            
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusDto updateStatusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _taskService.UpdateTaskStatusAsync(id, updateStatusDto);
            
            if (!result)
            {
                return NotFound(new { message = $"Task with ID {id} not found" });
            }
            
            return Ok(new { message = "Task status updated successfully" });
        }

        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasksByStatus(string status)
        {
            var tasks = await _taskService.GetTasksByStatusAsync(status);
            return Ok(tasks);
        }

        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetRecentTasks()
        {
            var tasks = await _taskService.GetTasksCreatedLastWeekAsync();
            return Ok(tasks);
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<Dictionary<string, int>>> GetTaskStatistics()
        {
            var statistics = await _taskService.GetTaskStatisticsAsync();
            return Ok(statistics);
        }
    }
}


