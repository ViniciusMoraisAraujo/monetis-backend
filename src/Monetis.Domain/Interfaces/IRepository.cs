namespace Monetis.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> GetByIdReadOnlyAsync(Guid id);
    Task<IReadOnlyList<T>> GetAllReadOnlyAsync();
    public void Create(T entity);
    public void Update(T entity);
    Task DeleteAsync(Guid id);
}

