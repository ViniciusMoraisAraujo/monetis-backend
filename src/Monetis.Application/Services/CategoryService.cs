using Microsoft.Extensions.Logging;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class CategoryService(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IUserContextAccessor userContextAccessor,
    ILogger<CategoryService> logger)
    : ICategoryService
{
    public async Task<CategoryResponse?> GetByIdAsync(Guid id)
    {
        logger.LogInformation("Getting category by id: {Id}", id);
        var category = await categoryRepository.GetByIdReadOnlyAsync(id);
        return category == null ? null : new CategoryResponse(category.Id, category.Name, category.UserId ?? Guid.Empty, category.Icon);
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        logger.LogInformation("Getting all categories");
        var categories = await categoryRepository.GetAllReadOnlyAsync();
        return categories.Select(c => new CategoryResponse(c.Id, c.Name, c.UserId ?? Guid.Empty, c.Icon));
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest createDto)
    {
        logger.LogInformation("Creating category: {Name}", createDto.Name);
        if (!userContextAccessor.IsResolved)
            throw new UnauthorizedAccessException("User context is not available.");

        var userId = userContextAccessor.UserId;
        var category = new Category(createDto.Name, userId, createDto.Icon);
        categoryRepository.Create(category);
        await unitOfWork.CommitAsync();
        return new CategoryResponse(category.Id, category.Name, category.UserId ?? Guid.Empty, category.Icon);
    }

    public async Task UpdateAsync(Guid id, UpdateCategoryRequest updateDto)
    {
        logger.LogInformation("Updating category: {Id}", id);
        var category = await categoryRepository.GetByIdAsync(id);
        if (category == null)
            throw new KeyNotFoundException($"Category with id {id} not found.");

        category.Update(updateDto.Name, updateDto.Icon);
        await unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        logger.LogInformation("Deleting category: {Id}", id);
        await categoryRepository.DeleteAsync(id);
        await unitOfWork.CommitAsync();
    }
}
