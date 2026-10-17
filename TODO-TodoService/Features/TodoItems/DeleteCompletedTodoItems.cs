using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TODO_TodoService.Data;

namespace TODO_TodoService.Features.TodoItems;

public static class DeleteCompletedTodoItems
{
    public static void MapDeleteCompletedTodoItems(this RouteGroupBuilder group)
    {
        group.MapDelete("/completed", HandleAsync).WithName("DeleteCompletedTodoItems");
    }

    internal static async Task<NoContent> HandleAsync(
        ClaimsPrincipal user, TodoDbContext db, CancellationToken cancellationToken)
    {
        var userId = CurrentUser.GetId(user);
        var completed = await db.TodoItems
            .Where(t => t.IsCompleted && t.UserId == userId)
            .ToListAsync(cancellationToken);

        db.TodoItems.RemoveRange(completed);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
