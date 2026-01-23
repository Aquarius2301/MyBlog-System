using Application.Exceptions;
using BusinessObject.Entities;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
/// <summary>
/// Authorization filter to check user status before allowing access to the endpoint.
/// </summary>
public class AuthorizeStatusAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string[] _requiredStatus;

    public AuthorizeStatusAttribute(params string[] requiredStatus)
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
        // Get the Account from HttpContext.Items (from UserValidationMiddleware)
        var account =
            context.HttpContext.Items["CurrentUser"] as Account
            ?? throw new UnauthorizedException("Unauthorized");

        // Check if the Status is in the allowed list
        if (!_requiredStatus.Contains(account.Status))
        {
            throw new ForbiddenException("Forbidden");
        }

        await Task.CompletedTask;
    }
}
