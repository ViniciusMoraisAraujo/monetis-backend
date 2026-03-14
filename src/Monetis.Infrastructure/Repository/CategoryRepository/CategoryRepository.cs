using Microsoft.EntityFrameworkCore;
using Monetis.Domain.Entities;
using Monetis.Infrastructure.Data;

namespace Monetis.Infrastructure.Repository.CategoryRepository;

public class CategoryRepository(MonetisDataContext context) : ICategoryRepository
{
    
    public Task<Category?> GetByIdReadOnlyAsync(Guid id)
        => context.Categories.FirstOrDefaultAsync(x => x.Id == id);


    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        var categories = await context.Categories.AsNoTracking().ToListAsync();
        return categories;
    }

    public async Task CreateAsync(Category entity)
    {
        await context.Categories.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category entity)
    {
        context.Categories.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var category =  await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if (category == null)
            throw new KeyNotFoundException();
            
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
    }
}