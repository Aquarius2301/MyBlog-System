using BusinessObject.Entities;

namespace DataAccess.Extensions;

public static class PostLikeExtension
{
    /// <summary>
    /// Filters PostLike by PostId
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="postId">The post ID to filter by</param>
    /// <returns></returns>
    public static IQueryable<PostLike> WherePostId(this IQueryable<PostLike> query, Guid postId)
    {
        return query.Where(pl => pl.PostId == postId);
    }

    /// <summary>
    /// Filters PostLike by AccountId
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="accountId">The account ID to filter by</param>
    /// <returns></returns>
    public static IQueryable<PostLike> WhereAccountId(
        this IQueryable<PostLike> query,
        Guid accountId
    )
    {
        return query.Where(pl => pl.AccountId == accountId);
    }
}
