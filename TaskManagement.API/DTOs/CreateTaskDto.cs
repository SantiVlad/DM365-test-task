namespace TaskManagement.API.DTOs
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public int? AssignedTo { get; set; }
    }
}


