namespace Monetis.Infrastructure.Repository;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdReadOnlyAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}

