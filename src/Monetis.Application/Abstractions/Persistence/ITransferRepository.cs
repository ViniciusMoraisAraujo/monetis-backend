using Monetis.Domain.Entities.Transactions;

namespace Monetis.Application.Abstractions.Persistence;

public interface ITransferRepository : IBaseRepository<Transfer>
{
    Task<Transfer?> GetByIdWithAccountsAsync(Guid id, CancellationToken cancellationToken = default);
}
