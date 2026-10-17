using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using TODO_TodoService.Data;

namespace TODO_TodoService.Features.TodoItems;

public static class GetTodoItemById
{
    public record Response(Guid Id, string Title, bool IsCompleted, DateTimeOffset CreatedAt);

    public static void MapGetTodoItemById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", HandleAsync).WithName("GetTodoItemById");
    }

    internal static async Task<Results<Ok<Response>, NotFound>> HandleAsync(
        Guid id, ClaimsPrincipal user, TodoDbContext db, CancellationToken cancellationToken)
    {
        var userId = CurrentUser.GetId(user);
        var item = await db.TodoItems.FindAsync([id], cancellationToken);
        if (item is null || item.UserId != userId)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new Response(item.Id, item.Title, item.IsCompleted, item.CreatedAt));
    }
}
