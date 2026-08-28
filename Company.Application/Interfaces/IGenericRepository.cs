using System.Linq.Expressions;

namespace Company.Application.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}