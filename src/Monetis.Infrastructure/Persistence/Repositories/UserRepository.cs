using Microsoft.EntityFrameworkCore;

using Monetis.Application.Abstractions.Persistence;
using Monetis.Domain.Entities;
using Monetis.Infrastructure.Persistence.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class UserRepository(MonetisDataContext context) : BaseRepository<User>(context), IUserRepository
{
    private readonly MonetisDataContext _context = context;

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await _context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

}
