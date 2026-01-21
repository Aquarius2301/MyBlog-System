using Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IActionResult HandleResponse<T>(ApiResponse<T> response)
        {
            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }

        protected ApiResponse<T> Success<T>(T? data = default, string message = "Success") =>
            new(200, message, data);

        protected ApiResponse<T> Created<T>(T? data = default, string message = "Success") =>
            new(201, message, data);

        protected ApiResponse<object> Error(int statusCode, string message) =>
            new(statusCode, message);
    }
}
