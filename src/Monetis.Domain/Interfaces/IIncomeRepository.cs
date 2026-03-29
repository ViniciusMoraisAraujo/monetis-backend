using Monetis.Domain.Entities.Transactions;

namespace Monetis.Domain.Interfaces;

public interface IIncomeRepository : IRepository<Income>
{
    Task<IEnumerable<Income>> GetByCategoryAsync(Guid categoryId); 
    Task<IEnumerable<Income>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Income>> GetByPeriodAsync(DateTime startDate, DateTime endDate, bool descending);
}