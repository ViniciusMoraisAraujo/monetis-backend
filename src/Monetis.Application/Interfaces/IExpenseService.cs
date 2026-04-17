using Monetis.Application.DTOs;
using Monetis.Domain.Entities.Transactions;

namespace Monetis.Application.Interfaces;

public interface IExpenseService
{
    Task<ExpenseResponse> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExpenseResponse>> CreateInstallmentAsync(CreateInstallmentRequest request, CancellationToken cancellationToken = default);
    
    Task<ExpenseResponse> PayExpenseAsync(Guid expenseId, PayExpenseRequest request, CancellationToken cancellationToken = default);
    Task<ExpenseResponse> PayInstallmentAsync(Guid expenseId, DateTime paidAt, CancellationToken cancellationToken = default);
    
    Task<ExpenseResponse> UpdateExpenseAsync(Guid expenseId, UpdateExpenseRequest request, CancellationToken cancellationToken = default);
    
    Task ProcessOverdueExpensesAsync(CancellationToken cancellationToken = default);
    
}