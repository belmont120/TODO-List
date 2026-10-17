using Microsoft.EntityFrameworkCore;
using TODO_TodoService.Models;

namespace TODO_TodoService.Data;

public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var demoUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var seededAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z");

        modelBuilder.Entity<TodoItem>().HasData(
            new TodoItem
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222221"),
                Title = "TODO Example",
                IsCompleted = false,
                CreatedAt = seededAt,
                UserId = demoUserId,
            },
            new TodoItem
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Title = "Completed TODO Example",
                IsCompleted = true,
                CreatedAt = seededAt,
                UserId = demoUserId,
            });
    }
}
