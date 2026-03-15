using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;
using Monetis.Infrastructure.Data;

namespace Monetis.Infrastructure.Repository;

public class CategoryRepository(MonetisDataContext context) : BaseRepository<Category>(context),ICategoryRepository
{
    
}