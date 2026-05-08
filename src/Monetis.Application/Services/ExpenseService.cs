using Monetis.Application.Abstractions.Persistence;
using Monetis.Application.Abstractions.Services;
using Monetis.Application.DTOs;
using Monetis.Domain.Entities.Transactions;

namespace Monetis.Application.Services;

public class ExpenseService(IExpenseRepository expenseRepository, 
    IUnitOfWork unitOfWork,
    IUserResourceGuard userResourceGuard) : IExpenseService
{
    public async Task<ExpenseResponse?> GetByIdAsync(Guid expenseId, CancellationToken cancellationToken = default)
    {
        var expense = await expenseRepository.GetByIdReadOnlyAsync(expenseId, cancellationToken);
        return expense == null ? null : MapToResponse(expense);
    }

    public async Task<IEnumerable<ExpenseResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var expenses = await expenseRepository.GetAllReadOnlyAsync(cancellationToken);
        return expenses.Select(MapToResponse);
    }

    public async Task<ExpenseResponse> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        var account = await userResourceGuard.GetOwnedAccountAsync(request.AccountId, cancellationToken);
        _ = await userResourceGuard.GetVisibleCategoryAsync(request.CategoryId, cancellationToken);
        await userResourceGuard.EnsureOptionalCardBelongsToUserAsync(request.CreditCardId, cancellationToken);
        
        var expense = new Expense(
            accountId: request.AccountId,
            categoryId: request.CategoryId,
            amount: request.Amount,
            description: request.Description,
            dueDate: request.DueDate,
            paymentMethod: request.PaymentMethod,
            creditCardId: request.CreditCardId);

        if (expense.IsPaidInCash)
        {
            account.Withdraw(expense.Amount);
            expense.Pay(DateTime.UtcNow, account.Id);
        }

        expenseRepository.Create(expense);
        await unitOfWork.CommitAsync(cancellationToken);

        return MapToResponse(expense);
    }

    public async Task<IReadOnlyCollection<ExpenseResponse>> CreateInstallmentAsync(CreateInstallmentRequest request, CancellationToken cancellationToken = default)
    {
        _ = await userResourceGuard.GetOwnedAccountAsync(request.AccountId, cancellationToken);
        _ = await userResourceGuard.GetVisibleCategoryAsync(request.CategoryId, cancellationToken);

        if (!request.CreditCardId.HasValue)
            throw new ArgumentException("Credit card is required for installments");

        _ = await userResourceGuard.GetOwnedCardAsync(request.CreditCardId.Value, cancellationToken);

        
        var installments = Expense.CreateInstallment(
            request.AccountId,
            request.CategoryId,
            request.TotalAmount,
            request.Description,
            request.FirstDueDate,
            request.NumberOfInstallments,
            request.CreditCardId.Value
        );
        
        foreach (var installment in installments)
        {
            expenseRepository.Create(installment);
        }

        await unitOfWork.CommitAsync(cancellationToken);

        return installments.Select(MapToResponse).ToList().AsReadOnly();
    }
 
    public async Task<ExpenseResponse> PayExpenseAsync(Guid expenseId, PayExpenseRequest request, CancellationToken cancellationToken = default)
    {
        var expense = await expenseRepository.GetByIdAsync(expenseId, cancellationToken);
        if (expense == null) throw new Exception("Expense not found");

        var targetAccountId = request.AccountId ?? expense.AccountId;
    
        var account = await userResourceGuard.GetOwnedAccountAsync(targetAccountId, cancellationToken);

        if (expense.IsPaidInCash || expense.IsInstallment)
        {
            account.Withdraw(expense.Amount);
        }

        expense.Pay(request.PaidAt, targetAccountId);

        expenseRepository.Update(expense);
        await unitOfWork.CommitAsync(cancellationToken);

        return MapToResponse(expense);
    }

    public async Task<ExpenseResponse> UpdateExpenseAsync(Guid expenseId, UpdateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        var expense = await expenseRepository.GetByIdAsync(expenseId, cancellationToken);
        if (expense == null) throw new Exception("Expense not found");
        _ = await userResourceGuard.GetVisibleCategoryAsync(request.CategoryId, cancellationToken);

        expense.Update(request.CategoryId, request.Amount, request.Description, request.DueDate);

        expenseRepository.Update(expense);
        await unitOfWork.CommitAsync(cancellationToken);

        return MapToResponse(expense);
    }
    

    public async Task ProcessOverdueExpensesAsync(CancellationToken cancellationToken = default)
    {
        var pendingExpenses = await expenseRepository.GetOverdueAsync(cancellationToken);
        foreach (var expense in pendingExpenses)
        {
            expense.MarkAsOverDue();
            expenseRepository.Update(expense);
        }
        await unitOfWork.CommitAsync(cancellationToken);
    }

    private static ExpenseResponse MapToResponse(Expense expense)
    {
        return new ExpenseResponse(
            expense.Id,
            expense.AccountId,
            expense.CategoryId,
            expense.Status,
            expense.PaidAt,
            expense.Amount,
            expense.Description,
            expense.DueDate,
            expense.PaymentMethod,
            expense.InstallmentNumber,
            expense.TotalInstallments,
            expense.InstallmentGroupId,
            expense.CreditCardId);
    }
}
