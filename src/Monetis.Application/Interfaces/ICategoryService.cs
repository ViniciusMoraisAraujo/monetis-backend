using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface ICategoryService
{
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto> CreateAsync(CreateCategoryDto createDto);
    Task UpdateAsync(Guid id, UpdateCategoryDto updateDto);
    Task DeleteAsync(Guid id);
}