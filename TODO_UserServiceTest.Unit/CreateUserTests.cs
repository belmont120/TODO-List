using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using TODO_UserService.Features.Users;
using TODO_UserService.Models;

namespace TODO_UserService.Tests.Unit;

public class CreateUserTests
{
    private readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    [Fact]
    public async Task CreatesUser_WithTrimmedFieldsAndHashedPassword()
    {
        await using var db = TestHelpers.CreateDbContext();

        var result = await CreateUser.HandleAsync(
            new CreateUser.Request("  jdoe  ", "  jdoe@example.com  ", "  Jane Doe  ", "Sup3rSecret!"),
            db,
            _passwordHasher,
            CancellationToken.None);

        var created = Assert.IsType<Created<CreateUser.Response>>(result.Result);
        Assert.Equal("jdoe", created.Value!.UserName);
        Assert.Equal("jdoe@example.com", created.Value.Email);
        Assert.Equal("Jane Doe", created.Value.DisplayName);

        var saved = Assert.Single(db.Users.Where(u => u.UserName == "jdoe"));
        Assert.NotEqual("Sup3rSecret!", saved.PasswordHash);
        Assert.Equal(
            PasswordVerificationResult.Success,
            _passwordHasher.VerifyHashedPassword(saved, saved.PasswordHash, "Sup3rSecret!"));
    }

    [Theory]
    [InlineData("", "jdoe@example.com", "123456")]
    [InlineData("jdoe", "", "123456")]
    [InlineData("jdoe", "jdoe@example.com", "short")]
    public async Task ReturnsValidationProblem_WhenRequiredFieldsAreInvalid(string userName, string email, string password)
    {
        await using var db = TestHelpers.CreateDbContext();

        var result = await CreateUser.HandleAsync(
            new CreateUser.Request(userName, email, "Display", password), db, _passwordHasher, CancellationToken.None);

        Assert.IsType<ValidationProblem>(result.Result);
    }

    [Fact]
    public async Task ReturnsConflict_WhenUserNameAlreadyExists()
    {
        await using var db = TestHelpers.CreateDbContext();

        var result = await CreateUser.HandleAsync(
            new CreateUser.Request("demo", "someone-else@example.com", "Someone Else", "Sup3rSecret!"),
            db,
            _passwordHasher,
            CancellationToken.None);

        Assert.IsType<Conflict<string>>(result.Result);
    }

    [Fact]
    public async Task ReturnsConflict_WhenEmailAlreadyExists()
    {
        await using var db = TestHelpers.CreateDbContext();

        var result = await CreateUser.HandleAsync(
            new CreateUser.Request("someoneelse", "demo@example.com", "Someone Else", "Sup3rSecret!"),
            db,
            _passwordHasher,
            CancellationToken.None);

        Assert.IsType<Conflict<string>>(result.Result);
    }
}
