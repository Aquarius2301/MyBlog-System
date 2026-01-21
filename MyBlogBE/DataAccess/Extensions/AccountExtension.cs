using System.Linq.Expressions;
using BusinessObject.Entities;
using BusinessObject.Enums;

namespace DataAccess.Extensions;

public static class AccountExtension
{
    /// <summary>
    /// Filter accounts by Id
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="id">The account Id to filter by</param>
    /// <returns></returns>
    public static IQueryable<Account> WhereId(this IQueryable<Account> query, Guid id)
    {
        return query.Where(a => a.Id == id);
    }

    /// <summary>
    /// Filter accounts by Username
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="username">The username to filter by</param>
    /// <returns></returns>
    public static IQueryable<Account> WhereUsername(this IQueryable<Account> query, string username)
    {
        return query.Where(a => a.Username == username);
    }

    /// <summary>
    /// Filter accounts by Username or Email
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="identifier">The username or email to filter by</param>
    /// <returns></returns>
    public static IQueryable<Account> WhereUsernameOrEmail(
        this IQueryable<Account> query,
        string identifier
    )
    {
        return query.Where(a => a.Username == identifier || a.Email == identifier);
    }

    /// <summary>
    /// Filter accounts where username contains the specified string
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="username">The username substring to filter by</param>
    /// <returns></returns>
    public static IQueryable<Account> WhereContainsUsername(
        this IQueryable<Account> query,
        string username
    )
    {
        return query.Where(a => a.Username != null && a.Username.Contains(username));
    }

    /// <summary>
    /// Filter accounts where displayname contains the specified string
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="displayname">The displayname substring to filter by</param>
    /// <returns></returns>
    public static IQueryable<Account> WhereContainsDisplayname(
        this IQueryable<Account> query,
        string displayname
    )
    {
        return query.Where(a => a.DisplayName != null && a.DisplayName.Contains(displayname));
    }

    public static IQueryable<Account> WhereContainsDisplaynameOrUsername(
        this IQueryable<Account> query,
        string name
    )
    {
        return query.Where(a =>
            (a.DisplayName != null && a.DisplayName.Contains(name))
            || (a.Username != null && a.Username.Contains(name))
        );
    }

    /// <summary>
    /// Filter accounts by Refresh Token
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="refreshToken">The refresh token to filter by</param>
    /// <returns></returns>
    public static IQueryable<Account> WhereRefreshToken(
        this IQueryable<Account> query,
        string refreshToken
    )
    {
        return query.Where(a => a.RefreshToken == refreshToken);
    }

    /// <summary>
    /// Filter accounts by Email
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="email">The email to filter by</param>
    /// <returns></returns>
    public static IQueryable<Account> WhereEmail(this IQueryable<Account> query, string email)
    {
        return query.Where(a => a.Email == email);
    }

    /// <summary>
    /// Filter accounts where CreatedAt is less than the specified cursor
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="cursor">The cursor to filter by</param>
    /// <returns></returns>
    /// <remarks>
    /// Use <c>OrderByDescending</c> for ordering the result after applying this filter.
    /// Make sure <c>cursor</c> is not null before calling this method.
    /// Use <c>WhereCursorGreaterThan</c> for ascending order pagination
    /// </remarks>
    /// <exception cref="ArgumentNullException">when cursor is null</exception>
    public static IQueryable<Account> WhereCursorLessThan(
        this IQueryable<Account> query,
        DateTime cursor
    )
    {
        return query.Where(a => a.CreatedAt < cursor);
    }

    /// <summary>
    /// Filter accounts where CreatedAt is greater than the specified cursor
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="cursor">The cursor to filter by</param>
    /// <returns></returns>
    /// <remarks>
    /// Use <c>OrderBy</c> for ordering the result after applying this filter.
    /// Make sure <c>cursor</c> is not null before calling this method.
    /// Use <c>WhereCursorLessThan</c> for descending order pagination
    /// </remarks>
    /// <exception cref="ArgumentNullException">when cursor is null</exception>
    public static IQueryable<Account> WhereCursorGreaterThan(
        this IQueryable<Account> query,
        DateTime cursor
    )
    {
        return query.Where(a => a.CreatedAt > cursor);
    }

    /// <summary>
    /// Filter accounts that are not soft-deleted
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <returns></returns>
    public static IQueryable<Account> WhereDeletedIsNull(this IQueryable<Account> query)
    {
        return query.Where(a => a.DeletedAt == null);
    }

    /// <summary>
    /// Filter accounts that have not initiated self-removal
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <returns></returns>
    public static IQueryable<Account> WhereSelfRemoveTimeIsNull(this IQueryable<Account> query)
    {
        return query.Where(a => a.SelfRemoveTime == null);
    }

    /// <summary>
    /// Filter accounts by Email Verified Code
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="code">The email verified code to filter by</param>
    /// <returns></returns>
    public static IQueryable<Account> WhereEmailVerifiedCode(
        this IQueryable<Account> query,
        string code
    )
    {
        return query.Where(a => a.EmailVerifiedCode == code);
    }

    /// <summary>
    /// Filter accounts by Verification Type
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="type">The verification type to filter by</param>
    /// <returns></returns>
    public static IQueryable<Account> WhereVerificationType(
        this IQueryable<Account> query,
        VerificationType type
    )
    {
        return query.Where(a => a.VerificationType == type);
    }

    /// <summary>
    /// Conditionally apply a where predicate to the query
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <param name="condition">The condition to apply the filter</param>
    /// <param name="apply">The filter predicate to apply if the condition is true</param>
    /// <returns></returns>
    /// <remarks>
    /// The <c>apply</c> parameter is a function from <c>AccountExtension</c> or a custom function that return <c>IQueryable&lt;Account&gt;</c>
    /// </remarks>
    public static IQueryable<Account> WhereIf(
        this IQueryable<Account> query,
        bool condition,
        Func<IQueryable<Account>, IQueryable<Account>> apply
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
    public static IQueryable<Account> WhereIf(
        this IQueryable<Account> query,
        bool condition,
        Expression<Func<Account, bool>> predicate
    )
    {
        return condition ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Filter accounts where SelfRemoveTime is less than the current time
    /// </summary>
    /// <param name="query">The query to filter</param>
    /// <returns></returns>
    public static IQueryable<Account> WhereSelfRemoveTimeIsLessThanNow(
        this IQueryable<Account> query
    )
    {
        return query.Where(a => a.SelfRemoveTime < DateTime.UtcNow);
    }
}
