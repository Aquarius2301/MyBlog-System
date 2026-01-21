using System.Net;
using System.Text.Json;
using Application.Dtos;
using Application.Exceptions;

namespace MyBlog.WebApi.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Default to 500 Internal Server Error
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = "Internal Server Error từ hệ thống.";

        // If the exception is a custom BaseException, use its status code and message
        if (exception is BaseException customEx)
        {
            statusCode = customEx.StatusCode;
            message = customEx.Message;
        }

        context.Response.StatusCode = statusCode;

        var response = new ApiResponse<object>(statusCode, message);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var json = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsync(json);
    }
}
