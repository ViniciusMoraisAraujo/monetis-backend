using Monetis.Domain.Entities.Transactions;
using Monetis.Domain.Enums;

namespace Monetis.Domain.Interfaces;

public interface IExpenseRepository : IBaseRepository<Expense>
{
    Task<IEnumerable<Expense>> GetByUserReadOnlyAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Expense>> GetByStatusReadOnlyAsync(TransactionStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Expense>> GetOverdueAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Expense>> GetByPeriodAsync(
        DateTime startDate,
        DateTime endDate,
        bool descending,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Expense>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

}
