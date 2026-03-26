using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Domain.Enums;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class CategoryRepository(MonetisDataContext context) : BaseRepository<Category>(context),ICategoryRepository
{
    public async Task<IEnumerable<Category>> GetAllTransactionTypeReadOnly(TransactionType type)
    {
        return await context.Categories.AsNoTracking().Where(c => c.Type == type).ToListAsync();
    }
}