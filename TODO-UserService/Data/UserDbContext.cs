using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TODO_UserService.Models;

namespace TODO_UserService.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.UserName).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        var hasher = new PasswordHasher<User>();
        var demoUser = new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            UserName = "demo",
            Email = "demo@example.com",
            DisplayName = "Demo User",
            PasswordHash = string.Empty,
            CreatedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
        };
        demoUser.PasswordHash = hasher.HashPassword(demoUser, "Demo123!");

        modelBuilder.Entity<User>().HasData(demoUser);
    }
}
