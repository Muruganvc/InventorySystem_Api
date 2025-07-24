using System.Linq.Expressions;

namespace InventorySystem_Domain.Common;

public interface IRepository<T> where T : class
{
    IQueryable<T> Table { get; }
    Task<IEnumerable<T>> GetAllAsync();
    Task<IReadOnlyList<T>> GetListByAsync(Expression<Func<T, bool>> predicate);
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void Delete(T entity);
}