using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface ICategoryService
{
    Task<CategoryResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<CategoryResponse>> GetAllAsync();
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest createDto);
    Task UpdateAsync(Guid id, UpdateCategoryRequest updateDto);
    Task DeleteAsync(Guid id);
}
