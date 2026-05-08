using Monetis.Domain.Entities.Transactions;

namespace Monetis.Domain.Interfaces;

public interface IIncomeRepository : IBaseRepository<Income>
{
    Task<IEnumerable<Income>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default); 
    Task<IEnumerable<Income>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Income>> GetByPeriodAsync(
        DateTime startDate,
        DateTime endDate,
        bool descending,
        CancellationToken cancellationToken = default);
}
