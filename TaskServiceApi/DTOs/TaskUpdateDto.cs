namespace TaskServiceApi.DTOs
{
    public class TaskUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateOnly DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}
