namespace InventorySystem_Domain.Common;
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveAsync();
    IRepository<T> Repository<T>() where T : class;
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken);
}
