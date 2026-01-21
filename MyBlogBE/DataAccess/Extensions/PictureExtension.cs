using BusinessObject.Entities;

namespace DataAccess.Extensions;

public static class PictureExtension
{
    /// <summary>
    /// Filter pictures by Id
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="id">The Id to filter by</param>
    /// <returns></returns>
    public static IQueryable<Picture> WhereId(this IQueryable<Picture> query, Guid id)
    {
        return query.Where(p => p.Id == id);
    }

    /// <summary>
    /// Filter pictures by Link
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="link">The link to filter by</param>
    /// <returns></returns>
    public static IQueryable<Picture> WhereLink(this IQueryable<Picture> query, string link)
    {
        return query.Where(p => p.Link == link);
    }

    /// <summary>
    /// Filter pictures by AccountId
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="id">The AccountId to filter by</param>
    /// <returns></returns>
    public static IQueryable<Picture> WhereAccountId(this IQueryable<Picture> query, Guid id)
    {
        return query.Where(p => p.AccountId == id);
    }

    /// <summary>
    /// Filter pictures by PostId
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="id">The PostId to filter by</param>
    /// <returns></returns>
    public static IQueryable<Picture> WherePostId(this IQueryable<Picture> query, Guid id)
    {
        return query.Where(p => p.PostId == id);
    }

    /// <summary>
    /// Filter pictures by CommentId
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="id">The CommentId to filter by</param>
    /// <returns></returns>
    public static IQueryable<Picture> WhereCommentId(this IQueryable<Picture> query, Guid id)
    {
        return query.Where(p => p.CommentId == id);
    }

    /// <summary>
    /// Filter pictures that are not associated with any Account, Post, or Comment
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <returns></returns>
    public static IQueryable<Picture> WhereIsNotBeUsed(this IQueryable<Picture> query)
    {
        return query.Where(p => p.AccountId == null && p.PostId == null && p.CommentId == null);
    }
}
