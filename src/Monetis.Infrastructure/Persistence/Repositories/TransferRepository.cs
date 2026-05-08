using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities.Transactions;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class TransferRepository(MonetisDataContext context)
    : BaseRepository<Transfer>(context), ITransferRepository
{
    public async Task<Transfer?> GetByIdWithAccountsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Set<Transfer>()
            .Include(x => x.Account)
            .Include(x => x.DestinationAccount)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
