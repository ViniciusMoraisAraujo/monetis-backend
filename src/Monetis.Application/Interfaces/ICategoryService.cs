using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface ICategoryService
{
    Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest createDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateCategoryRequest updateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
