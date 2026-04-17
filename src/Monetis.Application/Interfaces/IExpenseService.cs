using Monetis.Application.DTOs;

namespace Monetis.Application.Interfaces;

public interface IExpenseService
{
    Task<ExpenseResponse> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ExpenseResponse>> CreateInstallmentAsync(CreateInstallmentRequest request, CancellationToken cancellationToken = default);
    
    Task<ExpenseResponse> PayExpenseAsync(Guid expenseId, PayExpenseRequest request, CancellationToken cancellationToken = default);
    Task<ExpenseResponse> UpdateExpenseAsync(Guid expenseId, UpdateExpenseRequest request, CancellationToken cancellationToken = default);
    
    Task ProcessOverdueExpensesAsync(CancellationToken cancellationToken = default);
    
}