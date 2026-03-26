using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class BaseRepository<T>(MonetisDataContext context) : IRepository<T> where T : BaseEntity
{
    public async Task<T?> GetByIdAsync(Guid id)
        => await context.Set<T>().FindAsync(id);

    public async Task<T?> GetByIdReadOnlyAsync(Guid id)
        => await context.Set<T>().AsNoTracking().FirstOrDefaultAsync(x  => x.Id == id);

    public async Task<IReadOnlyList<T>> GetAllReadOnlyAsync()
        => await context.Set<T>().AsNoTracking().ToListAsync();

    public void Create(T entity)
        => context.Set<T>().Add(entity);

    public void Update(T entity)
    {
        context.Set<T>().Update(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            context.Set<T>().Remove(entity);
        }
    }
}