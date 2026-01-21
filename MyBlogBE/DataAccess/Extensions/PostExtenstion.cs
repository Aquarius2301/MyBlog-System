using System.Linq.Expressions;
using BusinessObject.Entities;

namespace DataAccess.Extensions;

public static class PostExtenstion
{
    /// <summary>
    /// Filter posts by Id
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="id">The post Id to filter by</param>
    /// <returns></returns>
    public static IQueryable<Post> WhereId(this IQueryable<Post> query, Guid id)
    {
        return query.Where(p => p.Id == id);
    }

    /// <summary>
    /// Filter posts that are not soft-deleted
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <returns></returns>
    public static IQueryable<Post> WhereDeletedIsNull(this IQueryable<Post> query)
    {
        return query.Where(p => p.DeletedAt == null);
    }

    /// <summary>
    /// Filter posts where CreatedAt is greater than the specified cursor
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
    public static IQueryable<Post> WhereCursorGreaterThan(
        this IQueryable<Post> query,
        DateTime cursor
    )
    {
        return query.Where(p => p.CreatedAt > cursor);
    }

    /// <summary>
    /// Filter posts where CreatedAt is less than the specified cursor
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
    public static IQueryable<Post> WhereCursorLessThan(this IQueryable<Post> query, DateTime cursor)
    {
        return query.Where(p => p.CreatedAt < cursor);
    }

    /// <summary>
    /// Filter posts by AccountId (the author of the post)
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="accountId">The AccountId to filter by</param>
    /// <returns></returns>
    public static IQueryable<Post> WhereAccountId(this IQueryable<Post> query, Guid accountId)
    {
        return query.Where(p => p.AccountId == accountId);
    }

    /// <summary>
    /// Filter posts by account's username (the author of the post)
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="username">The account's username to filter by</param>
    /// <returns></returns>
    public static IQueryable<Post> WhereAccountUsername(
        this IQueryable<Post> query,
        string username
    )
    {
        return query.Where(p => p.Account.Username == username);
    }

    /// <summary>
    /// Filter posts by Link
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="link">The link to filter by</param>
    /// <returns></returns>
    public static IQueryable<Post> WhereLink(this IQueryable<Post> query, string link)
    {
        return query.Where(p => p.Link == link);
    }

    /// <summary>
    /// Conditionally apply a where predicate to the query
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="condition">The condition to apply the filter</param>
    /// <param name="apply">The filter predicate to apply if the condition is true</param>
    /// <returns></returns>
    /// <remarks>
    /// The <c>apply</c> parameter is a function from <c>PostExtension</c> or a custom function that return <c>IQueryable&lt;Post&gt;</c>
    /// </remarks>
    public static IQueryable<Post> WhereIf(
        this IQueryable<Post> query,
        bool condition,
        Func<IQueryable<Post>, IQueryable<Post>> apply
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
    public static IQueryable<Post> WhereIf(
        this IQueryable<Post> query,
        bool condition,
        Expression<Func<Post, bool>> predicate
    )
    {
        return condition ? query.Where(predicate) : query;
    }
}
