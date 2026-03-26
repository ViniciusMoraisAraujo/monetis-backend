using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface ITransactionService
{
    Task<TransactionDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<TransactionDto>> GetAllAsync();
    Task<TransactionDto> CreateAsync(CreateTransactionDto createDto, Guid userId);
    Task UpdateAsync(Guid id, UpdateTransactionDto updateDto);
    Task DeleteAsync(Guid id);
}