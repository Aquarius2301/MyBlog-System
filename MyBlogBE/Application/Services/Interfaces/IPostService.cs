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
    /// <param name="accountId">The ID of the viewing user.</param>
    /// <param name="pageSize">The number of posts to return per request.</param>
    /// <returns>A list of <see cref="GetPostsResponse"/> and the cursor.</returns>
    Task<(List<GetPostsResponse>, DateTime?)> GetPostsListAsync(
        DateTime? cursor,
        Guid accountId,
        int pageSize
    );

    /// <summary>
    /// Gets a list of posts created by the current user.
    /// </summary>
    /// <param name="cursor">The timestamp of the last loaded post.</param>
    /// <param name="accountId">The ID of the user.</param>
    /// <param name="pageSize">The number of posts to return per request.</param>
    /// <returns>A list of <see cref="GetPostsResponse"/> of the owner and the cursor.</returns>
    Task<(List<GetPostsResponse>, DateTime?)> GetMyPostsListAsync(
        DateTime? cursor,
        Guid accountId,
        int pageSize
    );

    /// <summary>
    /// Gets a list of posts created by an user.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="cursor">The timestamp of the last loaded post.</param>
    /// <param name="accountId">The ID of the user.</param>
    /// <param name="pageSize">The number of posts to return per request.</param>
    /// <returns>A list of <see cref="GetPostsResponse"/> of the user and the cursor.</returns>
    Task<(List<GetPostsResponse>, DateTime?)> GetPostsByUsername(
        string username,
        DateTime? cursor,
        Guid accountId,
        int pageSize
    );

    /// <summary>
    /// Gets detailed information of a post by its link (slug).
    /// </summary>
    /// <param name="link">The slug of the post.</param>
    /// <param name="accountId">The ID of the user (used to check like status).</param>
    /// <returns>A <see cref="GetPostsResponse"/> contains detailed post information.</returns>
    Task<GetPostDetailResponse?> GetPostByLinkAsync(string link, Guid accountId);

    /// <summary>
    /// Likes a post for the given user.
    /// </summary>
    /// <param name="postId">The ID of the post to like.</param>
    /// <param name="accountId">The ID of the user performing the like.</param>
    /// <returns>
    /// The like number of the post, return null if not found the post
    /// </returns>
    Task<int?> LikePostAsync(Guid postId, Guid accountId);

    /// <summary>
    /// Cancels (removes) a like from a post.
    /// </summary>
    /// <param name="postId">The ID of the post to unlike.</param>
    /// <param name="accountId">The ID of the user performing the action.</param>
    /// <returns>
    /// The like number of the post, return null if not found the post
    /// </returns>
    Task<int?> CancelLikePostAsync(Guid postId, Guid accountId);

    /// <summary>
    /// Gets a paginated list of top-level comments for a post.
    /// </summary>
    /// <param name="postId">The ID of the post.</param>
    /// <param name="cursor">The timestamp of the last loaded comment (used for pagination).</param>
    /// <param name="accountId">The ID of the viewing user (used to check like status).</param>
    /// <param name="pageSize">The number of comments to load per request.</param>
    /// <returns>
    /// A list of <see cref="GetCommentsResponse"/> objects representing comments for the post and cursor.
    /// If return the list is null, that means the post is not found.
    /// </returns>
    Task<(List<GetCommentsResponse>?, DateTime?)> GetPostCommentsList(
        Guid postId,
        DateTime? cursor,
        Guid accountId,
        int pageSize
    );

    /// <summary>
    /// Adds a new post.
    /// </summary>
    /// <param name="request">The post creation request containing post details.</param>
    /// <param name="accountId">The ID of the account creating the post.</param>
    /// <returns>A <see cref="GetPostsResponse" objects representing created post></returns>
    Task<GetPostsResponse?> AddPostAsync(CreatePostRequest request, Guid accountId);

    /// <summary>
    /// Updates an existing post.
    /// </summary>
    /// <param name="request">The post update request containing updated post details.</param>
    /// <param name="postId">The ID of the post to update.</param>
    /// <param name="accountId">The ID of the account updating the post.</param
    /// <returns>
    /// An updated <see cref="GetPostsResponse"/> if the update is successful; otherwise, null.
    /// </returns>
    Task<GetPostsResponse?> UpdatePostAsync(UpdatePostRequest request, Guid postId, Guid accountId);

    /// <summary>
    /// Deletes a post.
    /// </summary>
    /// <param name="postId">The ID of the post to delete.</param>
    /// <param name="accountId">The ID of the account requesting the deletion.</param
    /// <returns>
    /// true if the post was successfully deleted; otherwise, false.
    /// </returns>
    Task<bool> DeletePostAsync(Guid postId, Guid accountId);
}
