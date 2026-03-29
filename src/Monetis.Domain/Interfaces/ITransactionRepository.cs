using Monetis.Domain.Entities;
using Monetis.Domain.Enums;

namespace Monetis.Domain.Interfaces;
public interface ITransactionRepository : IRepository<Transaction>
{ 
    Task<IEnumerable<Transaction>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Transaction>> GetAllByPeriodAsync(DateTime startDate, DateTime endDate, bool descending);
}