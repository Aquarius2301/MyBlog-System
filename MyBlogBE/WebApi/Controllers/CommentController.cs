using Application.Dtos;
using Application.Exceptions;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;
using WebApi.Helpers;

namespace WebApi.Controllers;

[Authorize]
[Route("api/comments")]
[ApiController]
public class CommentController : BaseController
{
    private readonly ICommentService _service;

    public CommentController(ICommentService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves a paginated list of child comments (replies) for a specific parent comment.
    /// </summary>
    /// <param name="id">The unique identifier of the parent comment.</param>
    /// <param name="request">Pagination parameters including cursor and page size.</param>
    /// <returns>
    /// 200 - Returns paginated list of child comments.
    /// 404 - Returns error if parent comment does not exist.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpGet("{id}")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> GetChildComments(
        Guid id,
        [FromQuery] PaginationRequest request
    )
    {
        var res = await _service.GetChildCommentList(id, request.Cursor, request.PageSize);

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
    /// Likes a specific comment.
    /// </summary>
    /// <param name="id">The unique identifier of the comment to like.</param>
    /// <returns>
    /// 200 - Returns success if comment is liked successfully.
    /// 404 - Returns error if comment does not exist.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpPost("{id}/like")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> LikeComment(Guid id)
    {
        var res = await _service.LikeCommentAsync(id);

        return HandleResponse(Success(res));
    }

    /// <summary>
    /// Removes a like from a specific comment.
    /// </summary>
    /// <param name="id">The unique identifier of the comment to unlike.</param>
    /// <returns>
    /// 200 - Returns success if like is removed successfully.
    /// 404 - Returns error if comment does not exist.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpDelete("{id}/cancel-like")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> CancelLikeComment(Guid id)
    {
        var res = await _service.CancelLikeCommentAsync(id);

        return HandleResponse(Success(res));
    }

    /// <summary>
    /// Adds a new comment.
    /// </summary>
    /// <param name="request">The comment creation request containing content and optional images.</param>
    /// <returns>
    /// 200 - Returns the created comment details.
    /// 400 - Returns error if request is invalid.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpPost]
    [AuthorizeStatusAttribute("active")]
    public async Task<IActionResult> AddComment([FromBody] CreateCommentRequest request)
    {
        // If ParentCommentId is provided, ReplyAccountId must also be provided, and vice versa
        if (request.ParentCommentId != null && request.ReplyAccountId == null)
            throw new BadRequestException("ReplyAccountRequired");

        if (request.ParentCommentId == null && request.ReplyAccountId != null)
            throw new BadRequestException("ParentCommentRequired");

        if (string.IsNullOrWhiteSpace(request.Content) && request.Pictures.Count == 0)
        {
            throw new BadRequestException("CommentAndPictureEmpty");
        }

        var res = await _service.AddCommentAsync(request);

        return HandleResponse(Success(res));
    }

    /// <summary>
    /// Updates an existing comment.
    /// </summary>
    /// <param name="id">The unique identifier of the comment to update.</param>
    /// <param name="request">The comment update request containing new content and optional images.</param>
    /// <returns>
    /// 200 - Returns the updated comment details.
    /// 400 - Returns error if request is invalid.
    /// 404 - Returns error if comment does not exist.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpPut("{id}")]
    [AuthorizeStatusAttribute("active")]
    public async Task<IActionResult> UpdateComment(Guid id, [FromForm] UpdateCommentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content) && request.Pictures.Count == 0)
        {
            throw new BadRequestException("CommentAndPictureEmpty");
        }

        var res = await _service.UpdateCommentAsync(id, request);

        return HandleResponse(Success(res));
    }

    /// <summary>
    /// Deletes a specific comment.
    /// </summary>
    /// <param name="id">The unique identifier of the comment to delete.</param>
    /// <returns>
    /// 200 - Returns success message if deletion is successful.
    /// 404 - Returns error if comment is not found.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpDelete("{id}")]
    [AuthorizeStatusAttribute(["active"])]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        await _service.DeleteCommentAsync(id);

        return HandleResponse(Success("CommentDeleted"));
    }
}
