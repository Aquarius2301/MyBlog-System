using Application.Dtos;
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
            var res = await _service.FollowUserAsync(followingId);
            return HandleResponse(Success(res, "FollowedSuccessfully"));
        }

        [HttpDelete("{followingId}")]
        public async Task<IActionResult> UnfollowUser(Guid followingId)
        {
            var res = await _service.UnfollowUserAsync(followingId);
            return HandleResponse(Success(res, "UnfollowedSuccessfully"));
        }

        [HttpGet("{userId}/followers")]
        public async Task<IActionResult> GetFollowers(
            Guid userId,
            [FromQuery] PaginationRequest request
        )
        {
            var res = await _service.GetFollowersAsync(userId, request.PageSize, request.Cursor);
            return HandleResponse(Success(res));
        }
    }
}
