using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/follows")]
    [ApiController]
    public class FollowController : BaseController
    {
        private readonly IFollowService _service;

        public FollowController(IFollowService service)
        {
            _service = service;
        }

        [HttpPost("{followingId}")]
        public async Task<IActionResult> FollowUser(Guid followingId)
        {
            await _service.FollowUserAsync(followingId);
            return HandleResponse(Success<object>(null, "FollowedSuccessfully"));
        }

        [HttpDelete("{followingId}")]
        public async Task<IActionResult> UnfollowUser(Guid followingId)
        {
            await _service.UnfollowUserAsync(followingId);
            return HandleResponse(Success<object>(null, "UnfollowedSuccessfully"));
        }
    }
}
