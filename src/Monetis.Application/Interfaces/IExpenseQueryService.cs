using Monetis.Domain.Entities.Transactions;

namespace Monetis.Application.Interfaces;

public interface IExpenseQueryService
{
    
    Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Expense>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Expense>> GetPendingByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
}