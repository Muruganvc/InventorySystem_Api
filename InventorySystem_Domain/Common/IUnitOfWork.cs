namespace InventorySystem_Domain.Common;
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveAsync();
    IRepository<T> Repository<T>() where T : class;
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken);
}
