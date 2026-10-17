namespace TODO_UserService.Models;

public class User
{
    public Guid Id { get; set; }

    public required string UserName { get; set; }

    public required string Email { get; set; }

    public required string DisplayName { get; set; }

    public required string PasswordHash { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
