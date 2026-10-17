using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using TODO_TodoService.Data;
using TODO_TodoService.Models;

namespace TODO_TodoService.Features.TodoItems;

public static class CreateTodoItem
{
    public record Request(string Title);

    public record Response(Guid Id, string Title, bool IsCompleted, DateTimeOffset CreatedAt);

    public static void MapCreateTodoItem(this RouteGroupBuilder group)
    {
        group.MapPost("/", HandleAsync).WithName("CreateTodoItem");
    }

    internal static async Task<Results<Created<Response>, ValidationProblem>> HandleAsync(
        Request request, ClaimsPrincipal user, TodoDbContext db, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["title"] = ["Title is required."],
            });
        }

        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            IsCompleted = false,
            UserId = CurrentUser.GetId(user),
        };

        db.TodoItems.Add(item);
        await db.SaveChangesAsync(cancellationToken);

        var response = new Response(item.Id, item.Title, item.IsCompleted, item.CreatedAt);
        return TypedResults.Created($"/api/todoitems/{item.Id}", response);
    }
}
