namespace Application.Dtos;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public ApiResponse(int statusCode, string message, T? data = default)
    {
        StatusCode = statusCode;
        Message = message;
        Data = data;
    }

    public static ApiResponse<T> Success(T? data = default, string message = "Success")
    {
        return new ApiResponse<T>(200, message, data);
    }

    // public static ApiResponse<T> Created(T? data = default, string message = "Created successfully")
    // {
    //     return new ApiResponse<T>(201, message, data);
    // }

    // public static IActionResult Conflict(string message = "Conflict", object? data = null)
    // {
    //     var res = new ApiResponse<object?>(409, message, data);
    //     return new ObjectResult(res) { StatusCode = 409 };
    // }

    // public static IActionResult BadRequest(string message = "Bad request", object? data = null)
    // {
    //     var res = new ApiResponse<object?>(400, message, data);
    //     return new ObjectResult(res) { StatusCode = 400 };
    // }

    // public static IActionResult Unauthorized(string message = "Unauthorized")
    // {
    //     var res = new ApiResponse<object?>(401, message, null);
    //     return new ObjectResult(res) { StatusCode = 401 };
    // }

    // public static IActionResult Forbidden(string message = "Forbidden")
    // {
    //     var res = new ApiResponse<object?>(403, message, null);
    //     return new ObjectResult(res) { StatusCode = 403 };
    // }

    // public static IActionResult NotFound(string message = "Not found")
    // {
    //     var res = new ApiResponse<object?>(404, message, null);
    //     return new ObjectResult(res) { StatusCode = 404 };
    // }

    // public static IActionResult Error(string message = "Internal server error", object? data = null)
    // {
    //     var res = new ApiResponse<object?>(500, message, data);
    //     return new ObjectResult(res) { StatusCode = 500 };
    // }

    // public static IActionResult Custom(int statusCode, string message, object? data = null)
    // {
    //     var res = new ApiResponse<object?>(statusCode, message, data);
    //     return new ObjectResult(res) { StatusCode = statusCode };
    // }
}
