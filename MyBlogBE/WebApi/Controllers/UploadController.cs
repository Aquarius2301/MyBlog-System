using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    public class FormRequest
    {
        public IFormFile[] Pictures { get; set; } = null!;
    }

    [Route("api/upload")]
    [ApiController]
    public class UploadController : BaseController
    {
        private readonly IUploadService _service;

        public UploadController(IUploadService uploadService)
        {
            _service = uploadService;
        }

        [HttpPost("")]
        [AuthorizeStatusAttribute("active")]
        public async Task<IActionResult> UploadPicture([FromForm] FormRequest request)
        {
            // Placeholder implementation for file upload
            // if (request == null)
            // {
            //     return BadRequest("No file uploaded.");
            // }

            var req = new List<IFileStream>();
            foreach (var file in request.Pictures)
            {
                req.Add(new FormFileHelper(file));
            }

            var res = await _service.UploadImageAsync([.. req]);

            return HandleResponse(Success(res));
        }
    }
}
