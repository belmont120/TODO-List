namespace TODO_TodoService.Features.TodoItems;

public static class TodoItemsEndpoints
{
    public static void MapTodoItemsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/todoitems")
            .RequireAuthorization()
            .WithTags("TodoItems");

        group.MapGetTodoItems();
        group.MapGetTodoItemById();
        group.MapCreateTodoItem();
        group.MapUpdateTodoItem();
        group.MapDeleteTodoItem();
        group.MapDeleteCompletedTodoItems();
    }
}
