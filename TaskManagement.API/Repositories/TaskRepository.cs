using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;
using TaskManagement.API.Models;

namespace TaskManagement.API.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public TaskRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("FirebirdConnection") ?? throw new InvalidOperationException("FirebirdConnection string is not configured");
        }

        private IDbConnection CreateConnection()
        {
            return new FbConnection(_connectionString);
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            using var connection = CreateConnection();
            var query = @"SELECT TASKID, TITLE, DESCRIPTION, STATUS, 
                         CREATEDBY, ASSIGNEDTO, CREATEDAT, UPDATEDAT 
                         FROM TASKS ORDER BY CREATEDAT DESC";
            return await connection.QueryAsync<TaskItem>(query);
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int taskId)
        {
            using var connection = CreateConnection();
            var query = @"SELECT TASKID, TITLE, DESCRIPTION, STATUS, 
                         CREATEDBY, ASSIGNEDTO, CREATEDAT, UPDATEDAT 
                         FROM TASKS WHERE TASKID = @TaskId";
            return await connection.QueryFirstOrDefaultAsync<TaskItem>(query, new { TaskId = taskId });
        }

        public async Task<int> CreateTaskAsync(TaskItem task)
        {
            using var connection = CreateConnection();
            var query = @"INSERT INTO TASKS (TITLE, DESCRIPTION, STATUS, CREATEDBY, ASSIGNEDTO, CREATEDAT) 
                         VALUES (@Title, @Description, @Status, @CreatedBy, @AssignedTo, @CreatedAt)
                         RETURNING TASKID";
            
            task.CreatedAt = DateTime.Now;
            return await connection.QuerySingleAsync<int>(query, task);
        }

        public async Task<bool> UpdateTaskAsync(TaskItem task)
        {
            using var connection = CreateConnection();
            var query = @"UPDATE TASKS SET 
                         TITLE = @Title, 
                         DESCRIPTION = @Description, 
                         STATUS = @Status, 
                         ASSIGNEDTO = @AssignedTo, 
                         UPDATEDAT = @UpdatedAt 
                         WHERE TASKID = @TaskId";
            
            task.UpdatedAt = DateTime.Now;
            var affectedRows = await connection.ExecuteAsync(query, task);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            using var connection = CreateConnection();
            var query = "DELETE FROM TASKS WHERE TASKID = @TaskId";
            var affectedRows = await connection.ExecuteAsync(query, new { TaskId = taskId });
            return affectedRows > 0;
        }

        public async Task<bool> UpdateTaskStatusAsync(int taskId, string status)
        {
            using var connection = CreateConnection();
            var query = @"UPDATE TASKS SET 
                         STATUS = @Status, 
                         UPDATEDAT = @UpdatedAt 
                         WHERE TASKID = @TaskId";
            
            var affectedRows = await connection.ExecuteAsync(query, 
                new { TaskId = taskId, Status = status, UpdatedAt = DateTime.Now });
            return affectedRows > 0;
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(string status)
        {
            using var connection = CreateConnection();
            var query = @"SELECT TASKID, TITLE, DESCRIPTION, STATUS, 
                         CREATEDBY, ASSIGNEDTO, CREATEDAT, UPDATEDAT 
                         FROM TASKS WHERE STATUS = @Status ORDER BY CREATEDAT DESC";
            return await connection.QueryAsync<TaskItem>(query, new { Status = status });
        }

        public async Task<IEnumerable<TaskItem>> GetTasksCreatedLastWeekAsync()
        {
            using var connection = CreateConnection();
            var query = @"SELECT TASKID, TITLE, DESCRIPTION, STATUS, 
                         CREATEDBY, ASSIGNEDTO, CREATEDAT, UPDATEDAT 
                         FROM TASKS 
                         WHERE CREATEDAT >= DATEADD(-7 DAY TO CURRENT_DATE)
                         ORDER BY CREATEDAT DESC";
            return await connection.QueryAsync<TaskItem>(query);
        }

        public async Task<Dictionary<string, int>> GetTaskCountByStatusAsync()
        {
            using var connection = CreateConnection();
            var query = @"SELECT STATUS, COUNT(*) AS COUNT 
                         FROM TASKS 
                         GROUP BY STATUS";
            var results = await connection.QueryAsync<dynamic>(query);
            return results.ToDictionary(x => (string)x.STATUS, x => (int)x.COUNT);
        }
    }
}


