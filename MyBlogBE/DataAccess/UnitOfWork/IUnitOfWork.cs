using BusinessObject.Entities;
using DataAccess.Repositories;

namespace DataAccess.UnitOfWork;

/// <summary>
/// Defines the unit of work interface for managing repositories and database transactions.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets the account repository.
    /// </summary>
    IRepository<Account> Accounts { get; }

    /// <summary>
    /// Gets the picture repository.
    /// </summary>
    IRepository<Picture> Pictures { get; }

    /// <summary>
    /// Gets the post repository.
    /// </summary>
    IRepository<Post> Posts { get; }

    /// <summary>
    /// Gets the post like repository.
    /// </summary>
    IRepository<PostLike> PostLikes { get; }

    /// <summary>
    /// Gets the comment repository.
    /// </summary>
    IRepository<Comment> Comments { get; }

    /// <summary>
    /// Gets the comment like repository.
    /// </summary>
    IRepository<CommentLike> CommentLikes { get; }

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <returns></returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits the current database transaction.
    /// </summary>
    /// <returns></returns>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rolls back the current database transaction.
    /// </summary>
    /// <returns></returns>
    Task RollbackTransactionAsync();

    /// <summary>
    /// Saves all changes made in the context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync();
}
