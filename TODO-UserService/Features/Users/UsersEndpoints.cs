namespace TODO_UserService.Features.Users;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapCreateUser();
    }
}
