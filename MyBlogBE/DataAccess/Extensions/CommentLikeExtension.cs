using BusinessObject.Entities;

namespace DataAccess.Extensions;

public static class CommentLikeExtension
{
    /// <summary>
    /// Filters CommentLike by CommentId
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="commentId">The comment ID to filter by</param>
    /// <returns></returns>
    public static IQueryable<CommentLike> WhereCommentId(
        this IQueryable<CommentLike> query,
        Guid commentId
    )
    {
        return query.Where(cl => cl.CommentId == commentId);
    }

    /// <summary>
    /// Filters CommentLike by AccountId
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="accountId">The account ID to filter by</param>
    /// <returns></returns>
    public static IQueryable<CommentLike> WhereAccountId(
        this IQueryable<CommentLike> query,
        Guid accountId
    )
    {
        return query.Where(cl => cl.AccountId == accountId);
    }
}
