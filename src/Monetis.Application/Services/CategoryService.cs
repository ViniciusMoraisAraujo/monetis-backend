using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdReadOnlyAsync(id);
        return category == null ? null : new CategoryDto(category.Id, category.Name, category.UserId ?? Guid.Empty, category.Type, category.Icon);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(c => new CategoryDto(c.Id, c.Name, c.UserId ?? Guid.Empty, c.Type, c.Icon));
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto createDto)
    {
        var category = new Category(createDto.Name, createDto.UserId, createDto.Type, createDto.Icon);
        await _categoryRepository.Create(category);
        await _unitOfWork.CommitAsync();
        return new CategoryDto(category.Id, category.Name, category.UserId ?? Guid.Empty, category.Type, category.Icon);
    }

    public async Task UpdateAsync(Guid id, UpdateCategoryDto updateDto)
    {
        var category = await _categoryRepository.GetByIdReadOnlyAsync(id);
        if (category == null)
            throw new KeyNotFoundException($"Category with id {id} not found.");

        category.Update(updateDto.Name, updateDto.Icon);
        
        _categoryRepository.Update(category);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _categoryRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}