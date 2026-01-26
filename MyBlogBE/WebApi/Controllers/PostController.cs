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

    public PostController(IPostService service)
    {
        _service = service;
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
        var res = await _service.GetPostsListAsync(request.Cursor, request.PageSize);

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
        var res = await _service.GetMyPostsListAsync(request.Cursor, request.PageSize);

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
    /// Retrieves a paginated list of posts created by a specific user.
    /// </summary>
    /// <param name="username">The username of the user whose posts are to be retrieved.</param>
    /// <param name="request">Pagination parameters including cursor and page size.</param>
    /// <returns>
    /// 200 - Returns paginated list of user's posts with cursor for next page.
    /// 404 - Returns error if user with specified username does not exist.
    /// 500 - Returns error message if exception occurs.
    [HttpGet("username/{username}")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> GetPostsByUsername(
        string username,
        [FromQuery] PaginationRequest request
    )
    {
        var res = await _service.GetPostsByUsernameAsync(
            username,
            request.Cursor,
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
        var res = await _service.GetPostByLinkAsync(link);

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
        var res = await _service.LikePostAsync(id);

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
        var res = await _service.CancelLikePostAsync(id);

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
        var res = await _service.GetPostCommentsListAsync(postId, request.Cursor, request.PageSize);

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
    /// 400 - Returns error if the post content and pictures are empty.
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

        var res = await _service.AddPostAsync(request);

        return HandleResponse(Created(res));
    }

    /// <summary>
    /// Updates an existing post.
    /// </summary>
    /// <param name="id">The unique identifier of the post to update.</param>
    /// <param name="request">The post update request containing updated post details.</param>
    /// <returns>
    /// 201 - Returns the updated post details upon successful update.
    /// 400 - Returns error if the post content and pictures are empty.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpPut("{id}")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> UpdatePost(Guid id, [FromBody] UpdatePostRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content) && request.Pictures.Count == 0)
        {
            throw new BadRequestException("PostAndPictureEmpty");
        }

        var res = await _service.UpdatePostAsync(request, id);

        return HandleResponse(Success(res));
    }

    /// <summary>
    /// Deletes a post by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the post to delete.</param>
    /// <returns>
    /// 200 - Returns success if the post is deleted successfully.
    /// 404 - Returns error if the post does not exist.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpDelete("{id}")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        await _service.DeletePostAsync(id);

        return HandleResponse(Success<object>(null));
    }
}
