using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Data;

namespace Monetis.Infrastructure.Repository;

public class BaseRepository<T>(MonetisDataContext context) : IRepository<T> where T : class
{

    public async Task<T?> GetByIdReadOnlyAsync(Guid id)
        => await context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);

    public async Task<IEnumerable<T>> GetAllAsync()
        => await context.Set<T>().AsNoTracking().ToListAsync();

    public async Task CreateAsync(T entity)
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
    }

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