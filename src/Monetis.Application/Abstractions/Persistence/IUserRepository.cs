using Monetis.Domain.Entities;

namespace Monetis.Application.Abstractions.Persistence;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
}
