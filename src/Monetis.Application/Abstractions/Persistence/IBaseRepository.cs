namespace Monetis.Application.Abstractions.Persistence;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<T?> GetByIdReadOnlyAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllReadOnlyAsync(CancellationToken cancellationToken = default);
    public void Create(T entity);
    public void Update(T entity);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
