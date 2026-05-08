using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Application.Abstractions.Persistence;
using Monetis.Infrastructure.Persistence.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class UserRepository(MonetisDataContext context) : BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    
}
