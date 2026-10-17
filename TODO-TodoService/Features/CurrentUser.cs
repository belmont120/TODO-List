using System.Security.Claims;

namespace TODO_TodoService.Features;

public static class CurrentUser
{
    public static Guid GetId(ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new InvalidOperationException("Missing user id claim."));
}
