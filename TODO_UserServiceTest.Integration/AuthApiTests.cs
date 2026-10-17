using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using TODO_UserService.Tests.Integration;

namespace TODO_UserServiceTest.Integration;

public class AuthApiTests(UserApiFactory factory) : IClassFixture<UserApiFactory>
{
    private record UserResponse(Guid Id, string UserName, string Email, string DisplayName, DateTimeOffset CreatedAt);

    private record LoginResponse(string Token, UserResponse User);

    [Fact]
    public async Task Login_WithValidDemoCredentials_ReturnsToken()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new { userName = "demo", password = "Demo123!" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.False(string.IsNullOrWhiteSpace(body!.Token));
        Assert.Equal("demo", body.User.UserName);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new { userName = "demo", password = "not-the-password" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithUnknownUser_ReturnsUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new { userName = "nobody", password = "whatever" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
