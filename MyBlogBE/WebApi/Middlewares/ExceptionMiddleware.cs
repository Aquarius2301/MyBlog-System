using System.Net;
using System.Text.Json;
using Application.Dtos;
using Application.Exceptions;

namespace WebApi.Middlewares;

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
        int statusCode;
        string? message;
        object? data = null;

        // If the exception is a custom BaseException, use its status code, message, and data
        switch (exception)
        {
            case BadRequestException badRequestEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = badRequestEx.Message;
                data = badRequestEx.Data;
                break;

            case NotFoundException notFoundEx:
                statusCode = (int)HttpStatusCode.NotFound;
                message = notFoundEx.Message;
                data = notFoundEx.Data;
                break;

            case ForbiddenException forbiddenEx:
                statusCode = (int)HttpStatusCode.Forbidden;
                message = forbiddenEx.Message;
                data = forbiddenEx.Data;
                break;

            case UnauthorizedException unauthorizedEx:
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = unauthorizedEx.Message;
                data = unauthorizedEx.Data;
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "Internal Server Error";
                break;
        }

        context.Response.StatusCode = statusCode;

        var response = new ApiResponse<object?>(statusCode, message, data);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        // var json = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsJsonAsync(response, options);
    }
}
