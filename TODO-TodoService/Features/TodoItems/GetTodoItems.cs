using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TODO_TodoService.Data;

namespace TODO_TodoService.Features.TodoItems;

public static class GetTodoItems
{
    public record Response(Guid Id, string Title, bool IsCompleted, DateTimeOffset CreatedAt);

    public static void MapGetTodoItems(this RouteGroupBuilder group)
    {
        group.MapGet("/", HandleAsync).WithName("GetTodoItems");
    }

    internal static async Task<Ok<List<Response>>> HandleAsync(
        ClaimsPrincipal user, TodoDbContext db, CancellationToken cancellationToken)
    {
        var userId = CurrentUser.GetId(user);

        var items = await db.TodoItems
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new Response(t.Id, t.Title, t.IsCompleted, t.CreatedAt))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(items);
    }
}
