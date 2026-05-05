using Monetis.Domain.Entities.Transactions;

namespace Monetis.Domain.Interfaces;

public interface ITransferRepository : IBaseRepository<Transfer>
{
    Task<Transfer?> GetByIdWithAccountsAsync(Guid id);
}
