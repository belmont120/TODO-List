using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TODO_TodoService.Data;

namespace TODO_TodoService.Features.TodoItems;

public static class DeleteTodoItem
{
    public static void MapDeleteTodoItem(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", HandleAsync).WithName("DeleteTodoItem");
    }

    internal static async Task<Results<Ok, NotFound>> HandleAsync(
        Guid id, ClaimsPrincipal user, TodoDbContext db, CancellationToken cancellationToken)
    {
        var userId = CurrentUser.GetId(user);
        var item = await db.TodoItems.SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        if (item is null)
        {
            return TypedResults.NotFound();
        }

        db.TodoItems.Remove(item);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
