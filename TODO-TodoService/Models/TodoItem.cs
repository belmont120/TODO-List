namespace TODO_TodoService.Models;

public class TodoItem
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public bool IsCompleted { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // The cross referencing of UserId between Todo and User services is achieved by JWT authentication
    public Guid? UserId { get; set; }
}
