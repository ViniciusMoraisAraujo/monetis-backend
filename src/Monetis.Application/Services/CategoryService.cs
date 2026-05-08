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
    public async Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting category by id: {Id}", id);
        var category = await categoryRepository.GetByIdReadOnlyAsync(id, cancellationToken);
        return category == null ? null : new CategoryResponse(category.Id, category.Name, category.UserId ?? Guid.Empty, category.Icon);
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all categories");
        var categories = await categoryRepository.GetAllReadOnlyAsync(cancellationToken);
        return categories.Select(c => new CategoryResponse(c.Id, c.Name, c.UserId ?? Guid.Empty, c.Icon));
    }

    public async Task<CategoryResponse> CreateAsync(
        CreateCategoryRequest createDto,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating category: {Name}", createDto.Name);
        if (!userContextAccessor.IsResolved)
            throw new UnauthorizedAccessException("User context is not available.");

        var userId = userContextAccessor.UserId;
        var category = new Category(createDto.Name, userId, createDto.Icon);
        categoryRepository.Create(category);
        await unitOfWork.CommitAsync(cancellationToken);
        return new CategoryResponse(category.Id, category.Name, category.UserId ?? Guid.Empty, category.Icon);
    }

    public async Task UpdateAsync(Guid id, UpdateCategoryRequest updateDto, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating category: {Id}", id);
        var category = await categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
            throw new KeyNotFoundException($"Category with id {id} not found.");

        category.Update(updateDto.Name, updateDto.Icon);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting category: {Id}", id);
        await categoryRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
