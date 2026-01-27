using Application.Dtos;

namespace Application.Services.Interfaces;

/// <summary>
/// Comment Service Interface
/// </summary>
public interface ICommentService
{
    /// <summary>
    /// Gets a paginated list of replies for a specific parent comment.
    /// </summary>
    /// <param name="commentId">The ID of the parent comment.</param>
    /// <param name="cursor">Timestamp of the last loaded child comment (used for pagination).</param>
    /// <param name="accountId">The ID of the viewing user (used to check like status).</param>
    /// <param name="pageSize">The number of replies to load per request.</param>
    /// <returns>
    /// A list of <see cref="GetChildCommentsResponse"/> objects and cursor
    /// The list is null if the parent comment does not exist.
    /// The cursor is null if there are no more child comments to load.
    /// </returns>
    /// <exception cref="NotFoundException">Thrown when the parent comment does not exist.</exception>
    Task<(List<GetCommentsResponse>, DateTime?)> GetChildCommentList(
        Guid commentId,
        DateTime? cursor,
        int pageSize
    );

    /// <summary>
    /// Likes a comment for the given user.
    /// </summary>
    /// <param name="commentId">The ID of the comment to like.</param>
    /// <returns>
    /// The number of likes after the like action if succeeded,
    /// </returns>
    /// <exception cref="NotFoundException">Thrown when the comment does not exist.</exception>
    Task<int> LikeCommentAsync(Guid commentId);

    /// <summary>
    /// Removes a like from a comment for the given user.
    /// </summary>
    /// <param name="commentId">The ID of the comment to unlike.</param>
    /// <returns>
    /// The number of likes after the unlike action if succeeded,
    /// </returns>
    /// <exception cref="NotFoundException">Thrown when the comment does not exist.</exception>
    Task<int> CancelLikeCommentAsync(Guid commentId);

    /// <summary>
    /// Adds a new comment.
    /// </summary>
    /// <param name="request">The details of the comment to add.</param>
    /// <returns> A <see cref="GetCommentsResponse"/> objects/returns>
    /// <exception cref="NotFoundException">Thrown when the post or parent comment does not exist.</exception>
    Task<GetCommentsResponse> AddCommentAsync(CreateCommentRequest request);

    /// <summary>
    /// Updates an existing comment.
    /// </summary>
    /// <param name="commentId">The ID of the comment to update.</param>
    /// <param name="request">The updated comment details.</param>
    /// <returns> A <see cref="GetCommentsResponse"/> objects/returns>
    /// <exception cref="NotFoundException">Thrown when the comment does not exist.</exception>
    Task<GetCommentsResponse> UpdateCommentAsync(Guid commentId, UpdateCommentRequest request);

    /// <summary>
    /// Deletes a comment.
    /// </summary>
    /// <param name="commentId">The ID of the comment to delete.</param>
    /// <returns>A task performs the deletion operation.</returns>
    /// <exception cref="NotFoundException">Thrown when the comment does not exist.</exception>
    Task DeleteCommentAsync(Guid commentId);
}
