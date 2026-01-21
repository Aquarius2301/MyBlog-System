using System.Linq.Expressions;
using BusinessObject.Entities;

namespace DataAccess.Extensions;

public static class CommentExtension
{
    /// <summary>
    /// Filter comments by Id
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="id">The comment Id to filter by</param>
    /// <returns></returns>
    public static IQueryable<Comment> WhereId(this IQueryable<Comment> query, Guid? id)
    {
        return query.Where(p => p.Id == id);
    }

    /// <summary>
    /// Filter comments by AccountId
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="id">The AccountId to filter by</param>
    /// <returns></returns>
    public static IQueryable<Comment> WhereAccountId(this IQueryable<Comment> query, Guid? id)
    {
        return query.Where(p => p.AccountId == id);
    }

    /// <summary>
    /// Filter comments that are not soft-deleted
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <returns></returns>
    public static IQueryable<Comment> WhereDeletedIsNull(this IQueryable<Comment> query)
    {
        return query.Where(p => p.DeletedAt == null);
    }

    /// <summary>
    /// Filter comments where CreatedAt is greater than the specified cursor
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="cursor">The cursor to compare CreatedAt against</param>
    /// <returns></returns>
    /// <remarks>
    /// Use <c>OrderBy</c> for ordering the result after applying this filter
    /// Make sure <c>cursor</c> is not null before calling this method.
    /// Use <c>WhereCursorLessThan</c> for descending order pagination
    /// </remarks>
    /// <exception cref="ArgumentNullException">when cursor is null</exception>
    public static IQueryable<Comment> WhereCursorGreaterThan(
        this IQueryable<Comment> query,
        DateTime cursor
    )
    {
        return query.Where(p => p.CreatedAt > cursor);
    }

    /// <summary>
    /// Filter comments where CreatedAt is less than the specified cursor
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="cursor">The cursor to compare CreatedAt against</param>
    /// <returns></returns>
    /// <remarks>
    /// Use <c>OrderByDescending</c> for ordering the result after applying this filter.
    /// Make sure <c>cursor</c> is not null before calling this method.
    /// Use <c>WhereCursorGreaterThan</c> for ascending order pagination
    /// </remarks>
    /// <exception cref="ArgumentNullException">when cursor is null</exception>
    public static IQueryable<Comment> WhereCursorLessThan(
        this IQueryable<Comment> query,
        DateTime cursor
    )
    {
        return query.Where(p => p.CreatedAt < cursor);
    }

    public static IQueryable<Comment> WherePostId(this IQueryable<Comment> query, Guid id)
    {
        return query.Where(p => p.PostId == id);
    }

    public static IQueryable<Comment> WhereParentId(this IQueryable<Comment> query, Guid? id)
    {
        return query.Where(p => p.ParentCommentId == id);
    }

    public static IQueryable<Comment> WhereReplyAccountId(this IQueryable<Comment> query, Guid? id)
    {
        return query.Where(p => p.ReplyAccountId == id);
    }

    /// <summary>
    /// Conditionally apply a where predicate to the query
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="condition">The condition to apply the filter</param>
    /// <param name="apply">The filter predicate to apply if the condition is true</param>
    /// <returns></returns>
    /// <remarks>
    /// The <c>apply</c> parameter is a function from <c>CommentExtension</c> or a custom function that return <c>IQueryable&lt;Comment&gt;</c>
    /// </remarks>
    public static IQueryable<Comment> WhereIf(
        this IQueryable<Comment> query,
        bool condition,
        Func<IQueryable<Comment>, IQueryable<Comment>> apply
    )
    {
        return condition ? apply(query) : query;
    }

    /// <summary>
    /// Conditionally apply a where predicate to the query
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="condition">The condition to apply the filter</param>
    /// <param name="predicate">The filter predicate to apply if the condition is true</param>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    public static IQueryable<Comment> WhereIf(
        this IQueryable<Comment> query,
        bool condition,
        Expression<Func<Comment, bool>> predicate
    )
    {
        return condition ? query.Where(predicate) : query;
    }
}
