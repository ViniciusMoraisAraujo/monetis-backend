using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class BaseRepository<T>(MonetisDataContext context) : IBaseRepository<T> where T : BaseEntity
{
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<T?> GetByIdReadOnlyAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<T>> GetAllReadOnlyAsync(CancellationToken cancellationToken = default)
        => await context.Set<T>().AsNoTracking().ToListAsync(cancellationToken);

    public void Create(T entity)
        => context.Set<T>().Add(entity);

    public void Update(T entity)
        => context.Set<T>().Update(entity);

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        if (entity != null)
        {
            context.Set<T>().Remove(entity);
        }
    }
}
