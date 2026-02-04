using Application.Dtos;

namespace Application.Services.Interfaces;

/// <summary>
/// Post Service Interface
/// </summary>
public interface IPostService
{
    /// <summary>
    /// Gets a list of posts using cursor-based pagination (infinite scroll style).
    /// </summary>
    /// <param name="cursor">
    /// The timestamp of the last loaded post.
    /// If null → fetch the newest posts.
    /// </param>
    /// <param name="pageSize">The number of posts to return per request.</param>
    /// <returns>A list of <see cref="GetPostsResponse"/> and the cursor.</returns>
    Task<(List<GetPostsResponse>, DateTime?)> GetPostsListAsync(DateTime? cursor, int pageSize);

    /// <summary>
    /// Gets a list of posts created by the current user.
    /// </summary>
    /// <param name="cursor">The timestamp of the last loaded post.</param>
    /// <param name="pageSize">The number of posts to return per request.</param>
    /// <returns>A list of <see cref="GetPostsResponse"/> of the owner and the cursor.</returns>
    Task<(List<GetPostsResponse>, DateTime?)> GetMyPostsListAsync(DateTime? cursor, int pageSize);

    /// <summary>
    /// Gets a list of posts created by an user.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="cursor">The timestamp of the last loaded post.</param>
    /// <param name="pageSize">The number of posts to return per request.</param>
    /// <returns>A list of <see cref="GetPostsResponse"/> of the user and the cursor.</returns>
    /// <exception cref="NotFoundException">Thrown when the user is not found.</exception>
    Task<(List<GetPostsResponse>, DateTime?)> GetPostsByUsernameAsync(
        string username,
        DateTime? cursor,
        int pageSize
    );

    /// <summary>
    /// Gets detailed information of a post by its link (slug).
    /// </summary>
    /// <param name="link">The slug of the post.</param>
    /// <returns>A <see cref="GetPostsResponse"/> contains detailed post information.</returns>
    /// <exception cref="NotFoundException">Thrown when the post is not found.</exception>
    Task<GetPostDetailResponse> GetPostByLinkAsync(string link);

    /// <summary>
    /// Likes a post for the given user.
    /// </summary>
    /// <param name="postId">The ID of the post to like.</param>
    /// <returns>
    /// The like number of the post
    /// </returns>
    /// <exception cref="NotFoundException">Thrown when the post is not found.</exception>
    Task<int> LikePostAsync(Guid postId);

    /// <summary>
    /// Cancels (removes) a like from a post.
    /// </summary>
    /// <param name="postId">The ID of the post to unlike.</param>
    /// <returns>
    /// The like number of the post, return null if not found the post
    /// </returns>
    /// <exception cref="NotFoundException">Thrown when the post is not found.</exception>
    Task<int> CancelLikePostAsync(Guid postId);

    /// <summary>
    /// Gets a paginated list of top-level comments for a post.
    /// </summary>
    /// <param name="postId">The ID of the post.</param>
    /// <param name="cursor">The timestamp of the last loaded comment (used for pagination).</param>
    /// <param name="pageSize">The number of comments to load per request.</param>
    /// <returns>
    /// A list of <see cref="GetCommentsResponse"/> objects representing comments for the post and cursor.
    /// </returns>
    /// <exception cref="NotFoundException">Thrown when the post is not found.</exception>
    Task<(List<GetCommentsResponse>, DateTime?)> GetPostCommentsListAsync(
        Guid postId,
        DateTime? cursor,
        int pageSize
    );

    /// <summary>
    /// Adds a new post.
    /// </summary>
    /// <param name="request">The post creation request containing post details.</param>
    /// <returns>A <see cref="GetPostsResponse" objects representing created post></returns>
    /// <exception cref="NotFoundException">Thrown when the account is not found.</exception>
    Task<GetPostsResponse> AddPostAsync(CreatePostRequest request);

    /// <summary>
    /// Updates an existing post.
    /// </summary>
    /// <param name="request">The post update request containing updated post details.</param>
    /// <param name="postId">The ID of the post to update.</param>
    /// <returns>
    /// An updated <see cref="GetPostsResponse"/> if the update is successful; otherwise, null.
    /// </returns>
    /// <exception cref="NotFoundException">Thrown when the post is not found.</exception>
    Task<GetPostsResponse> UpdatePostAsync(UpdatePostRequest request, Guid postId);

    /// <summary>
    /// Deletes a post.
    /// </summary>
    /// <param name="postId">The ID of the post to delete.</param>
    /// <returns>
    /// A task that represents the asynchronous delete operation.
    /// </returns>
    /// <exception cref="NotFoundException">Thrown when the post is not found.</exception>
    /// <remarks>
    /// The post is soft-deleted by setting its DeletedAt property.
    /// Warnings: This operation only unlink pictures associations in the database, but does not delete the comments
    /// </remarks>
    Task DeletePostAsync(Guid postId);
}
