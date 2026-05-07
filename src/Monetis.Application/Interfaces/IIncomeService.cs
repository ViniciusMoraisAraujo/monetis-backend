using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface IIncomeService
{
    Task<IncomeResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<IncomeResponse>> GetAllAsync();
    Task<IncomeResponse> CreateAsync(CreateIncomeRequest request);
    Task UpdateAsync(Guid id, UpdateIncomeRequest request);
    Task DeleteAsync(Guid id);
}
