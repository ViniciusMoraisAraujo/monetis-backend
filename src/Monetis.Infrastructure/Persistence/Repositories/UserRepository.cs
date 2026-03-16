using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class UserRepository(MonetisDataContext context) : BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> GetUserByEmailAsync(string email)
        => await context.Users.FirstOrDefaultAsync(x => x.Email == email);
    
}