using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.API.DTOs;
using TaskManagement.API.Models;
using TaskManagement.API.Repositories;

namespace TaskManagement.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllTasksAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int taskId)
        {
            return await _taskRepository.GetTaskByIdAsync(taskId);
        }

        public async Task<TaskItem?> CreateTaskAsync(CreateTaskDto createTaskDto)
        {
            var taskItem = new TaskItem
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                Status = createTaskDto.Status,
                CreatedBy = createTaskDto.CreatedBy,
                AssignedTo = createTaskDto.AssignedTo
            };

            var taskId = await _taskRepository.CreateTaskAsync(taskItem);
            return await _taskRepository.GetTaskByIdAsync(taskId);
        }

        public async Task<TaskItem?> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto)
        {
            var existingTask = await _taskRepository.GetTaskByIdAsync(taskId);
            if (existingTask == null)
                return null;

            existingTask.Title = updateTaskDto.Title;
            existingTask.Description = updateTaskDto.Description;
            existingTask.Status = updateTaskDto.Status;
            existingTask.AssignedTo = updateTaskDto.AssignedTo;

            var success = await _taskRepository.UpdateTaskAsync(existingTask);
            if (success)
                return await _taskRepository.GetTaskByIdAsync(taskId);
            
            return null;
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            return await _taskRepository.DeleteTaskAsync(taskId);
        }

        public async Task<bool> UpdateTaskStatusAsync(int taskId, UpdateTaskStatusDto updateStatusDto)
        {
            return await _taskRepository.UpdateTaskStatusAsync(taskId, updateStatusDto.Status);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(string status)
        {
            return await _taskRepository.GetTasksByStatusAsync(status);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksCreatedLastWeekAsync()
        {
            return await _taskRepository.GetTasksCreatedLastWeekAsync();
        }

        public async Task<Dictionary<string, int>> GetTaskStatisticsAsync()
        {
            return await _taskRepository.GetTaskCountByStatusAsync();
        }
    }
}


