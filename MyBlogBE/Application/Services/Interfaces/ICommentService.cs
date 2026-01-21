using Application.Dtos;

namespace Application.Services.Interfaces;

/// <summary>
/// Comment Service Interface
/// </summary>
public interface ICommentService
{
    // /// <summary>
    // /// Gets a comment by its unique identifier.
    // /// </summary>
    // /// <param name="commentId">The unique identifier of the comment.</param>
    // /// <returns> A <see cref="Comment"/> with the specified ID if found; otherwise, null.</returns>
    // Task<Comment?> GetByIdAsync(Guid commentId);

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
    Task<(List<GetCommentsResponse>?, DateTime?)> GetChildCommentList(
        Guid commentId,
        DateTime? cursor,
        Guid accountId,
        int pageSize
    );

    /// <summary>
    /// Likes a comment for the given user.
    /// </summary>
    /// <param name="commentId">The ID of the comment to like.</param>
    /// <param name="accountId">The ID of the user performing the like.</param>
    /// <returns>
    /// The number of likes after the like action if succeeded,
    /// Null if the comment does not exist.
    /// </returns>
    Task<int?> LikeCommentAsync(Guid commentId, Guid accountId);

    /// <summary>
    /// Removes a like from a comment for the given user.
    /// </summary>
    /// <param name="commentId">The ID of the comment to unlike.</param>
    /// <param name="accountId">The ID of the user performing the action.</param>
    /// <returns>
    /// The number of likes after the unlike action if succeeded,
    /// Null if the comment does not exist.
    /// </returns>
    Task<int?> CancelLikeCommentAsync(Guid commentId, Guid accountId);

    /// <summary>
    /// Adds a new comment.
    /// </summary>
    /// <param name="accountId">The ID of the user adding the comment.</param>
    /// <param name="request">The details of the comment to add.</param>
    /// <returns> A <see cref="GetCommentsResponse"/> objects, null if failed</returns>
    Task<GetCommentsResponse?> AddCommentAsync(Guid accountId, CreateCommentRequest request);

    /// <summary>
    /// Updates an existing comment.
    /// </summary>
    /// <param name="commentId">The ID of the comment to update.</param>
    /// <param name="request">The updated comment details.</param>
    /// <param name="accountId">The ID of the user performing the update.</param>
    /// <returns> A <see cref="GetCommentsResponse"/> objects, null if failed</returns>
    Task<GetCommentsResponse?> UpdateCommentAsync(
        Guid commentId,
        UpdateCommentRequest request,
        Guid accountId
    );

    /// <summary>
    /// Deletes a comment.
    /// </summary>
    /// <param name="commentId">The ID of the comment to delete.</param>
    /// <param name="accountId">The ID of the user performing the deletion.</param
    /// <returns> True if deletion succeeded, false otherwise.</returns>
    Task<bool> DeleteCommentAsync(Guid commentId, Guid accountId);
}
