using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Domain.Enums;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Data;

namespace Monetis.Infrastructure.Repository;

public class CategoryRepository(MonetisDataContext context) : BaseRepository<Category>(context),ICategoryRepository
{
    public async Task<IEnumerable<Category>> GetAllTransactionType(TransactionType type)
    {
        return await context.Categories.AsNoTracking().Where(c => c.Type == type).ToListAsync();
    }
}