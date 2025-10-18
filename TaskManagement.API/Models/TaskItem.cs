using System;

namespace TaskManagement.API.Models
{
    public class TaskItem
    {
        public int TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public int? AssignedTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}


