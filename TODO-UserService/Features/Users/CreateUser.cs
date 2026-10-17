using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TODO_UserService.Data;
using TODO_UserService.Models;

namespace TODO_UserService.Features.Users;

public static class CreateUser
{
    public record Request(string UserName, string Email, string DisplayName, string Password);

    public record Response(Guid Id, string UserName, string Email, string DisplayName, DateTimeOffset CreatedAt);

    public static void MapCreateUser(this RouteGroupBuilder group)
    {
        group.MapPost("/", HandleAsync).WithName("CreateUser");
    }

    public static async Task<Results<Created<Response>, ValidationProblem, Conflict<string>>> HandleAsync(
        Request request, UserDbContext db, IPasswordHasher<User> passwordHasher, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            errors["userName"] = ["UserName is required."];
        }
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors["email"] = ["Email is required."];
        }
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
        {
            errors["password"] = ["Password must be at least 6 characters."];
        }
        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var exists = await db.Users.AnyAsync(
            u => u.UserName == request.UserName.Trim() || u.Email == request.Email.Trim(), cancellationToken);
        if (exists)
        {
            return TypedResults.Conflict("A user with the same username or email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName.Trim(),
            Email = request.Email.Trim(),
            DisplayName = request.DisplayName.Trim(),
            PasswordHash = string.Empty,
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        var response = new Response(user.Id, user.UserName, user.Email, user.DisplayName, user.CreatedAt);
        return TypedResults.Created($"/api/users/{user.Id}", response);
    }
}
