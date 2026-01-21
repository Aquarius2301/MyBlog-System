using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class Repository<T> : IRepository<T>
    where T : class
{
    private readonly MyBlogContext _context;

    public Repository(MyBlogContext context)
    {
        _context = context;
    }

    public IQueryable<T> GetQuery()
    {
        return _context.Set<T>();
    }

    public IQueryable<T> ReadOnly()
    {
        return _context.Set<T>().AsNoTracking();
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _context.Set<T>().AddRange(entities);
    }

    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
    }
}
