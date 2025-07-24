using InventorySystem_Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventorySystem_Infrastructure;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly InventorySystemDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(InventorySystemDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    public IQueryable<T> Table => _dbSet.AsQueryable();
    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    public async Task<IReadOnlyList<T>> GetListByAsync(Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();
    public async Task<T?> GetByAsync(Expression<Func<T, bool>> predicate) => await _dbSet.FirstOrDefaultAsync(predicate);
    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    public async Task AddRangeAsync(IEnumerable<T> entities) => await _dbSet.AddRangeAsync(entities);
    public void Update(T entity) => _dbSet.Update(entity);
    public void Delete(T entity) => _dbSet.Remove(entity);
}
