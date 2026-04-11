using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class CardRepository(MonetisDataContext context) : BaseRepository<Card>(context), ICardRepository
{
    public async Task<IEnumerable<Card>> GetByUserReadOnlyAsync(Guid userId)
    {
        return await context.Cards
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }
}
