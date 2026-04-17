using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities.Transactions;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class ExpenseService(IExpenseRepository expenseRepository, 
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork) : IExpenseService
{
    public async Task<ExpenseResponse> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId);
        if (account == null || account.UserId != request.UserId)
            throw new Exception("Account not found or access denied");
        
        var expense = new Expense(
            userId: request.UserId,
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
        var account = await accountRepository.GetByIdAsync(request.AccountId);
        if (account == null || account.UserId != request.UserId)
            throw new Exception("Account not found or access denied");

        if (!request.CreditCardId.HasValue)
            throw new ArgumentException("Credit card is required for installments");

        
        var installments = Expense.CreateInstallment(
            request.UserId,
            request.AccountId,
            request.CategoryId,
            request.TotalAmount,
            request.Description,
            request.FirstDueDate,
            request.NumberOfInstallments,
            request.PaymentMethod,
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
        var expense = await expenseRepository.GetByIdAsync(expenseId);
        if (expense == null) throw new Exception("Expense not found");

        var targetAccountId = request.AccountId ?? expense.AccountId;
    
        var account = await accountRepository.GetByIdAsync(targetAccountId);
        if (account == null) throw new Exception("Account not found");

        if (expense.IsPaidInCash || expense.IsInstallment)
        {
            account.Withdraw(expense.Amount);
        }

        expense.Pay(request.PaidAt, targetAccountId);

        expenseRepository.Update(expense);
        await unitOfWork.CommitAsync(cancellationToken);

        return MapToResponse(expense);
    }

    public Task<ExpenseResponse> PayInstallmentAsync(Guid expenseId, DateTime paidAt, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ExpenseResponse> UpdateExpenseAsync(Guid expenseId, UpdateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        var expense = await expenseRepository.GetByIdAsync(expenseId);
        if (expense == null) throw new Exception("Expense not found");

        expense.Update(request.CategoryId, request.Amount, request.Description, request.DueDate);

        expenseRepository.Update(expense);
        await unitOfWork.CommitAsync(cancellationToken);

        return MapToResponse(expense);
    }
    

    public async Task ProcessOverdueExpensesAsync(CancellationToken cancellationToken = default)
    {
        var pendingExpenses = await expenseRepository.GetOverdueAsync();
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
