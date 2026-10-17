using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TODO_UserService.Data;
using TODO_UserService.Models;
using TODO_UserService.Services;

namespace TODO_UserService.Features.Auth;

public static class Login
{
    public record Request(string UserName, string Password);

    public record UserResponse(Guid Id, string UserName, string Email, string DisplayName, DateTimeOffset CreatedAt);

    public record Response(string Token, UserResponse User);

    public static void MapLogin(this RouteGroupBuilder group)
    {
        group.MapPost("/login", HandleAsync).WithName("Login");
    }

    public static async Task<Results<Ok<Response>, UnauthorizedHttpResult>> HandleAsync(
        Request request,
        UserDbContext db,
        IPasswordHasher<User> passwordHasher,
        JwtTokenService tokenService,
        CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName, cancellationToken);
        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return TypedResults.Unauthorized();
        }

        var token = tokenService.CreateToken(user);
        var response = new Response(
            token,
            new UserResponse(user.Id, user.UserName, user.Email, user.DisplayName, user.CreatedAt));

        return TypedResults.Ok(response);
    }
}
