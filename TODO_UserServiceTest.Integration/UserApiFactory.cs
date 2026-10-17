using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TODO_UserService.Data;

namespace TODO_UserService.Tests.Integration;

/// <summary>
/// Boots the real UserService pipeline (auth, CORS, endpoints) against an isolated in-memory database.
/// </summary>
public class UserApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"UserServiceDb-Tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UserDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<UserDbContext>(options => options.UseInMemoryDatabase(_databaseName));
        });
    }
}
