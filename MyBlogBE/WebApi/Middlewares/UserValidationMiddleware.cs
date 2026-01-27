using System.Security.Claims;
using DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Middlewares;

public class UserValidationMiddleware
{
    private readonly RequestDelegate _next;

    public UserValidationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
    {
        // Only validate if the user is authenticated
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
        {
            var account = await unitOfWork
                .Accounts.ReadOnly()
                .Where(a => a.Id == userId && a.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (account == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Unauthorized" });
                return;
            }

            // Store in HttpContext.Items for Filter and Service reuse
            context.Items["CurrentUser"] = account;
        }

        await _next(context);
    }
}
