using Company.Application.Interfaces;
using Company.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Company.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly CompanyDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(CompanyDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Loads an entity by its id together with the given navigation paths.
    /// Paths use EF's dotted syntax, e.g. "ArticleTags" or "ArticleTags.Tag".
    /// </summary>
    public async Task<T?> GetByIdIncludingAsync(
        int id,
        params string[] includePaths)
    {
        IQueryable<T> query = _dbSet;

        foreach (var path in includePaths)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                query = query.Include(path);
            }
        }

        // All domain entities extend BaseEntity, which exposes the int Id key.
        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

}