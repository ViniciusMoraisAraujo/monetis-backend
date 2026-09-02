using Microsoft.EntityFrameworkCore;

using Monetis.Application.Abstractions.Persistence;
using Monetis.Domain.Entities;
using Monetis.Infrastructure.Persistence.Contexts;

namespace Monetis.Infrastructure.Persistence.Repositories;

public class CategoryRepository(MonetisDataContext context) : BaseRepository<Category>(context), ICategoryRepository
{
}
