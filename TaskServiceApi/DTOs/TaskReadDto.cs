namespace TaskServiceApi.DTOs
{
    public class TaskReadDto
    {
        public int Id { get; init; }
        public string Title { get; init; } = "";
        public string? Description { get; init; }
        public DateOnly DueDate { get; init; }
        public bool IsCompleted { get; init; }
    }
}
