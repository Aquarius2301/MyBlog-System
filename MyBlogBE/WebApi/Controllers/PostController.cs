using Application.Dtos;
using Application.Exceptions;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Authorize]
[Route("api/posts")]
[ApiController]
public class PostController : BaseController
{
    private readonly IPostService _service;
    private readonly IJwtService _jwtService;

    public PostController(IPostService service, IJwtService jwtService)
    {
        _service = service;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Retrieves a paginated list of all posts in the system.
    /// </summary>
    /// <param name="request">Pagination parameters including cursor and page size.</param>
    /// <returns>
    /// 200 - Returns paginated list of posts with cursor for next page.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpGet("")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> GetPosts([FromQuery] PaginationRequest request)
    {
        var user = _jwtService.GetAccountInfo();

        var res = await _service.GetPostsListAsync(request.Cursor, user.Id, request.PageSize);

        return HandleResponse(
            Success(
                new PaginationResponse
                {
                    Items = res.Item1,
                    Cursor = res.Item2,
                    PageSize = request.PageSize,
                }
            )
        );
    }

    /// <summary>
    /// Retrieves a paginated list of posts created by the authenticated user.
    /// </summary>
    /// <param name="request">Pagination parameters including cursor and page size.</param>
    /// <returns>
    /// 200 - Returns paginated list of user's own posts with cursor for next page.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpGet("me")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> GetMyPosts([FromQuery] PaginationRequest request)
    {
        var user = _jwtService.GetAccountInfo();

        var res = await _service.GetMyPostsListAsync(request.Cursor, user.Id, request.PageSize);

        return HandleResponse(
            Success(
                new PaginationResponse
                {
                    Items = res.Item1,
                    Cursor = res.Item2,
                    PageSize = request.PageSize,
                }
            )
        );
    }

    [HttpGet("username/{username}")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> GetPostsByUsername(
        string username,
        [FromQuery] PaginationRequest request
    )
    {
        var user = _jwtService.GetAccountInfo();

        var res = await _service.GetPostsByUsername(
            username,
            request.Cursor,
            user.Id,
            request.PageSize
        );

        return HandleResponse(
            Success(
                new PaginationResponse
                {
                    Items = res.Item1,
                    Cursor = res.Item2,
                    PageSize = request.PageSize,
                }
            )
        );
    }

    /// <summary>
    /// Retrieves a specific post by its unique link identifier.
    /// </summary>
    /// <param name="link">The unique link/slug identifier of the post.</param>
    /// <returns>
    /// 200 - Returns the post details if found.
    /// 404 - Returns error if post with specified link does not exist.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpGet("link/{link}")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> GetPostsByLink(string link)
    {
        var user = _jwtService.GetAccountInfo();

        var res = await _service.GetPostByLinkAsync(link, user.Id);

        return HandleResponse(Success(res));
    }

    /// <summary>
    /// Likes a specific post.
    /// </summary>
    /// <param name="id">The unique identifier of the post to like.</param>
    /// <returns>
    /// 200 - Returns success if post is liked successfully.
    /// 404 - Returns error if post does not exist.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpPost("{id}/like")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> LikePost(Guid id)
    {
        var user = _jwtService.GetAccountInfo();

        var res = await _service.LikePostAsync(id, user.Id);

        return HandleResponse(Success(res));
    }

    /// <summary>
    /// Removes a like from a specific post.
    /// </summary>
    /// <param name="id">The unique identifier of the post to unlike.</param>
    /// <returns>
    /// 200 - Returns success if like is removed successfully.
    /// 404 - Returns error if post does not exist.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpDelete("{id}/cancel-like")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> CancelLikePost(Guid id)
    {
        var user = _jwtService.GetAccountInfo();

        var res = await _service.CancelLikePostAsync(id, user.Id);

        return HandleResponse(Success(res));
    }

    /// <summary>
    /// Retrieves a paginated list of comments for a specific post.
    /// </summary>
    /// <param name="postId">The unique identifier of the post.</param>
    /// <param name="request">Pagination parameters including cursor and page size.</param>
    /// <returns>
    /// 200 - Returns paginated list of comments with cursor for next page.
    /// 404 - Returns error if post does not exist.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpGet("{postId}/comments")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> GetPostComments(
        Guid postId,
        [FromQuery] PaginationRequest request
    )
    {
        var user = _jwtService.GetAccountInfo();
        var res = await _service.GetPostCommentsList(
            postId,
            request.Cursor,
            user.Id,
            request.PageSize
        );

        return HandleResponse(
            Success(
                new PaginationResponse
                {
                    Items = res.Item1,
                    Cursor = res.Item2,
                    PageSize = request.PageSize,
                }
            )
        );
    }

    /// <summary>
    /// Creates a new post for the authenticated user.
    /// </summary>
    /// <param name="request">The post creation request containing content.</param>
    /// <returns>
    /// 201 - Returns the created post details upon successful creation.
    /// 400 - Returns error if the post content is empty.
    /// 403 - Returns error if the user's account status does not allow posting.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpPost("")]
    [AuthorizeStatusAttribute("active")]
    public async Task<IActionResult> AddPost([FromBody] CreatePostRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content) && request.Pictures.Count == 0)
        {
            throw new BadRequestException("PostAndPictureEmpty");
        }

        var user = _jwtService.GetAccountInfo();

        var res = await _service.AddPostAsync(request, user.Id);

        return HandleResponse(Created(res));
    }

    [HttpPut("{id}")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> UpdatePost(Guid id, [FromBody] UpdatePostRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content) || request.Pictures.Count == 0)
        {
            throw new BadRequestException("PostAndPictureEmpty");
        }

        var user = _jwtService.GetAccountInfo();

        var res = await _service.UpdatePostAsync(request, id, user.Id);

        return HandleResponse(Success(res));
    }

    [HttpDelete("{id}")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var user = _jwtService.GetAccountInfo();

        var res = await _service.DeletePostAsync(id, user.Id);

        return HandleResponse(Success<object>(null));
    }
}
