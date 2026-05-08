using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Application.Abstractions.Persistence;
using Monetis.Infrastructure.Persistence.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class CardRepository(MonetisDataContext context) : BaseRepository<Card>(context), ICardRepository
{
    public async Task<IEnumerable<Card>> GetByUserReadOnlyAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Cards
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}
