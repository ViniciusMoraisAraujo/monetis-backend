using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface IIncomeService
{
    Task<IncomeResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<IncomeResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IncomeResponse> CreateAsync(CreateIncomeRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateIncomeRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
