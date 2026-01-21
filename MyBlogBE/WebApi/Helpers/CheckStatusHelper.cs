using Application.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Helpers;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
/// <summary>
/// Authorization filter to check user status before allowing access to the endpoint.
/// </summary>
public class CheckStatusHelper : Attribute, IAsyncAuthorizationFilter
{
    private readonly string[] _requiredStatus;

    public CheckStatusHelper(string requiredStatus)
    {
        _requiredStatus = [requiredStatus];
    }

    public CheckStatusHelper(string[] requiredStatus)
    {
        _requiredStatus = requiredStatus;
    }

    /// <summary>
    /// Checks the user's status and authorizes access accordingly.
    /// </summary>
    /// <param name="context">The authorization filter context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            throw new UnauthorizedException("Unauthorized");
        }

        var claimValue = user.Claims.FirstOrDefault(c => c.Type == "Status")?.Value;

        if (!_requiredStatus.Contains(claimValue))
        {
            throw new ForbiddenException("Forbidden");
        }

        await Task.CompletedTask;
    }
}
