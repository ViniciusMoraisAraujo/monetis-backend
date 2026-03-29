using Monetis.Domain.Entities.Transactions;
using Monetis.Domain.Enums;

namespace Monetis.Domain.Interfaces;

public interface IExpenseRepository : IRepository<Expense>
{
    Task<IEnumerable<Expense>> GetByUserReadOnlyAsync(Guid userId);
    Task<IEnumerable<Expense>> GetByStatusReadOnlyAsync(TransactionStatus status);
    Task<IEnumerable<Expense>> GetOverdueAsync();
    Task<IEnumerable<Expense>> GetByPeriodAsync(DateTime startDate, DateTime endDate, bool descending);
    Task<IEnumerable<Expense>> GetByCategoryAsync(Guid categoryId);

}