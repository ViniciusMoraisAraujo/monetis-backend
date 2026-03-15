using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Data;

namespace Monetis.Infrastructure.Repository;

public class UserRepository(MonetisDataContext context) : BaseRepository<User>(context),IUserRepository
{
    public async Task<User?> GetByIdReadOnlyAsync(Guid id)
        => await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IEnumerable<User>> GetAllAsync()
        => await context.Users.AsNoTracking().ToListAsync();

    public async Task CreateAsync(User entity)
    {
        context.Users.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User entity)
    {
        context.Users.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
            throw new KeyNotFoundException();

        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }
}