namespace DataAccess.Repositories;

/// <summary>
/// Generic repository interface for managing entities of type <typeparamref name="T"/>.
/// </summary>
public interface IRepository<T>
    where T : class
{
    /// <summary>
    /// Returns an <see cref="IQueryable{T}"/> for the entity, allowing for further LINQ compositions.
    /// Can be used for insert, update, delete, and read operations.
    /// </summary>
    /// <returns>An <see cref="IQueryable{T}"/> of the requested entity type.</returns>
    IQueryable<T> GetQuery();

    /// <summary>
    /// Returns an <see cref="IQueryable{T}"/> specifically for read-only operations.
    /// Don't use it for insert, update, or delete operations.
    /// </summary>
    /// <returns>A non-tracking <see cref="IQueryable{T}"/>.</returns>
    IQueryable<T> ReadOnly();

    /// <summary>
    /// Marks a new entity to be inserted into the data store.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void Add(T entity);

    /// <summary>
    /// Marks a collection of entities to be inserted into the data store.
    /// </summary>
    /// <param name="entities">The collection of entities to add.</param>
    void AddRange(IEnumerable<T> entities);

    /// <summary>
    /// Marks an existing entity to be deleted from the data store.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(T entity);

    /// <summary>
    /// Marks a collection of existing entities to be deleted from the data store.
    /// </summary>
    /// <param name="entities">The collection of entities to remove.</param>
    void RemoveRange(IEnumerable<T> entities);
}
