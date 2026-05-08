using Monetis.Application.DTOs;

namespace Monetis.Application.Abstractions.Services;

public interface IExpenseService
{
    Task<ExpenseResponse?> GetByIdAsync(Guid expenseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExpenseResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ExpenseResponse> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ExpenseResponse>> CreateInstallmentAsync(CreateInstallmentRequest request, CancellationToken cancellationToken = default);
    
    Task<ExpenseResponse> PayExpenseAsync(Guid expenseId, PayExpenseRequest request, CancellationToken cancellationToken = default);
    Task<ExpenseResponse> UpdateExpenseAsync(Guid expenseId, UpdateExpenseRequest request, CancellationToken cancellationToken = default);
    
    Task ProcessOverdueExpensesAsync(CancellationToken cancellationToken = default);
    
}
