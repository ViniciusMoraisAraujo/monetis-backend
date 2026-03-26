using Monetis.Domain.Entities;
using Monetis.Domain.Enums;

namespace Monetis.Domain.Interfaces;
public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>>  GetAllTransactionTypeReadOnly(TransactionType type);
}