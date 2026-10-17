using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using TODO_UserService.Features.Auth;
using TODO_UserService.Models;

namespace TODO_UserService.Tests.Unit;

public class LoginTests
{
    private readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    [Fact]
    public async Task ReturnsTokenAndUser_ForValidCredentials()
    {
        await using var db = TestHelpers.CreateDbContext();
        var tokenService = TestHelpers.CreateTokenService();

        var result = await Login.HandleAsync(
            new Login.Request("demo", "Demo123!"), db, _passwordHasher, tokenService, CancellationToken.None);

        var ok = Assert.IsType<Ok<Login.Response>>(result.Result);
        Assert.False(string.IsNullOrWhiteSpace(ok.Value!.Token));
        Assert.Equal("demo", ok.Value.User.UserName);
    }

    [Fact]
    public async Task ReturnsUnauthorized_ForWrongPassword()
    {
        await using var db = TestHelpers.CreateDbContext();
        var tokenService = TestHelpers.CreateTokenService();

        var result = await Login.HandleAsync(
            new Login.Request("demo", "wrong-password"), db, _passwordHasher, tokenService, CancellationToken.None);

        Assert.IsType<UnauthorizedHttpResult>(result.Result);
    }

    [Fact]
    public async Task ReturnsUnauthorized_ForUnknownUser()
    {
        await using var db = TestHelpers.CreateDbContext();
        var tokenService = TestHelpers.CreateTokenService();

        var result = await Login.HandleAsync(
            new Login.Request("nobody", "whatever"), db, _passwordHasher, tokenService, CancellationToken.None);

        Assert.IsType<UnauthorizedHttpResult>(result.Result);
    }
}
