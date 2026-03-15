namespace Monetis.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdReadOnlyAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task CreateAsync(T entity);
    public void Update(T entity);
    Task DeleteAsync(Guid id);
}

