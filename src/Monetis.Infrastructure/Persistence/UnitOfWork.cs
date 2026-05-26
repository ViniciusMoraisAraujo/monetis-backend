using Microsoft.EntityFrameworkCore;
using Monetis.Application.Abstractions.Persistence;
using Monetis.Application.Abstractions.Security;
using Monetis.Domain.Entities;
using Monetis.Infrastructure.Persistence.Contexts;

namespace Monetis.Infrastructure.Persistence;

public class UnitOfWork(MonetisDataContext monetisDataContext, IUserContextAccessor userContext) : IUnitOfWork
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
    {
        var entries = monetisDataContext.ChangeTracker
            .Entries<UserOwnedEntity>()
            .Where(e => e.State == EntityState.Added);

        foreach (var entry in entries)
        {
            entry.Entity.SetUser(userContext.UserId);
        }

        return await monetisDataContext.SaveChangesAsync(cancellationToken) > 0;
    }
}