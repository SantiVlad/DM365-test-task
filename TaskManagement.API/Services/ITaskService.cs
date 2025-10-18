using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.API.DTOs;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetTaskByIdAsync(int taskId);
        Task<TaskItem?> CreateTaskAsync(CreateTaskDto createTaskDto);
        Task<TaskItem?> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto);
        Task<bool> DeleteTaskAsync(int taskId);
        Task<bool> UpdateTaskStatusAsync(int taskId, UpdateTaskStatusDto updateStatusDto);
        Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(string status);
        Task<IEnumerable<TaskItem>> GetTasksCreatedLastWeekAsync();
        Task<Dictionary<string, int>> GetTaskStatisticsAsync();
    }
}


