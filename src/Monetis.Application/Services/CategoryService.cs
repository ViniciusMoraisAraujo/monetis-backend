using Microsoft.Extensions.Logging;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class CategoryService(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ILogger<CategoryService> logger)
    : ICategoryService
{
    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        logger.LogInformation("Getting category by id: {Id}", id);
        var category = await categoryRepository.GetByIdReadOnlyAsync(id);
        return category == null ? null : new CategoryDto(category.Id, category.Name, category.UserId ?? Guid.Empty, category.Type, category.Icon);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        logger.LogInformation("Getting all categories");
        var categories = await categoryRepository.GetAllAsync();
        return categories.Select(c => new CategoryDto(c.Id, c.Name, c.UserId ?? Guid.Empty, c.Type, c.Icon));
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto createDto)
    {
        logger.LogInformation("Creating category: {Name}", createDto.Name);
        var category = new Category(createDto.Name, createDto.UserId, createDto.Type, createDto.Icon);
        await categoryRepository.Create(category);
        await unitOfWork.CommitAsync();
        return new CategoryDto(category.Id, category.Name, category.UserId ?? Guid.Empty, category.Type, category.Icon);
    }

    public async Task UpdateAsync(Guid id, UpdateCategoryDto updateDto)
    {
        logger.LogInformation("Updating category: {Id}", id);
        var category = await categoryRepository.GetByIdReadOnlyAsync(id);
        if (category == null)
            throw new KeyNotFoundException($"Category with id {id} not found.");

        category.Update(updateDto.Name, updateDto.Icon);
        
        categoryRepository.Update(category);
        await unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        logger.LogInformation("Deleting category: {Id}", id);
        await categoryRepository.DeleteAsync(id);
        await unitOfWork.CommitAsync();
    }
}
