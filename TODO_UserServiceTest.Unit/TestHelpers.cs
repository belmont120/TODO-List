using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TODO_UserService.Data;
using TODO_UserService.Services;

namespace TODO_UserService.Tests.Unit;

internal static class TestHelpers
{
    public static UserDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new UserDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static JwtTokenService CreateTokenService()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "TodoListApp.UserService.Tests",
                ["Jwt:Audience"] = "TodoListApp.Tests",
                ["Jwt:Key"] = "unit-test-only-signing-key-not-for-production-use!",
                ["Jwt:ExpiryMinutes"] = "30",
            })
            .Build();

        return new JwtTokenService(configuration);
    }
}
