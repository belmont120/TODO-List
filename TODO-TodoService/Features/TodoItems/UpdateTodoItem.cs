using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TODO_TodoService.Data;

namespace TODO_TodoService.Features.TodoItems;

public static class UpdateTodoItem
{
    public record Request(string Title, bool IsCompleted);

    public record Response(Guid Id, string Title, bool IsCompleted, DateTimeOffset CreatedAt);

    public static void MapUpdateTodoItem(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", HandleAsync).WithName("UpdateTodoItem");
    }

    internal static async Task<Results<Ok<Response>, NotFound, ValidationProblem>> HandleAsync(
        Guid id, Request request, ClaimsPrincipal user, TodoDbContext db, CancellationToken cancellationToken)
    {
        var userId = CurrentUser.GetId(user);
        var item = await db.TodoItems.SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        if (item is null)
        {
            return TypedResults.NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["title"] = ["Title is required."],
            });
        }

        item.Title = request.Title.Trim();
        item.IsCompleted = request.IsCompleted;
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(new Response(item.Id, item.Title, item.IsCompleted, item.CreatedAt));
    }
}
