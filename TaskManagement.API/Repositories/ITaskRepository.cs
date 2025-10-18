using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.API.Models;

namespace TaskManagement.API.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetTaskByIdAsync(int taskId);
        Task<int> CreateTaskAsync(TaskItem task);
        Task<bool> UpdateTaskAsync(TaskItem task);
        Task<bool> DeleteTaskAsync(int taskId);
        Task<bool> UpdateTaskStatusAsync(int taskId, string status);
        Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(string status);
        Task<IEnumerable<TaskItem>> GetTasksCreatedLastWeekAsync();
        Task<Dictionary<string, int>> GetTaskCountByStatusAsync();
    }
}


