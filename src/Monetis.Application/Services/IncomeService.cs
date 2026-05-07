using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities.Transactions;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class IncomeService(
    IIncomeRepository incomeRepository,
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    IUserContextAccessor userContextAccessor)
    : IIncomeService
{
    public async Task<IncomeResponse?> GetByIdAsync(Guid id)
    {
        var income = await incomeRepository.GetByIdReadOnlyAsync(id);
        return income == null ? null : MapToResponse(income);
    }

    public async Task<IEnumerable<IncomeResponse>> GetAllAsync()
    {
        var incomes = await incomeRepository.GetAllReadOnlyAsync();
        return incomes.Select(MapToResponse);
    }

    public async Task<IncomeResponse> CreateAsync(CreateIncomeRequest request)
    {
        if (!userContextAccessor.IsResolved)
            throw new UnauthorizedAccessException("User context is not available.");

        var account = await accountRepository.GetByIdAsync(request.AccountId);
        if (account == null || account.UserId != userContextAccessor.UserId)
            throw new Exception("Account not found or access denied");

        var income = Income.CreatePaid(
            request.AccountId,
            request.CategoryId,
            request.Amount,
            request.Description,
            request.ReceivedAt);

        account.Deposit(income.Amount);
        incomeRepository.Create(income);
        await unitOfWork.CommitAsync();

        return MapToResponse(income);
    }

    public async Task UpdateAsync(Guid id, UpdateIncomeRequest request)
    {
        var income = await incomeRepository.GetByIdAsync(id);
        if (income == null)
            throw new KeyNotFoundException($"Income with id {id} not found.");

        var account = await accountRepository.GetByIdAsync(income.AccountId);
        if (account == null)
            throw new KeyNotFoundException($"Account with id {income.AccountId} not found.");

        var amountDelta = request.Amount - income.Amount;
        if (amountDelta > 0)
            account.Deposit(amountDelta);
        else if (amountDelta < 0)
            account.Withdraw(Math.Abs(amountDelta));

        income.Update(
            request.CategoryId,
            request.Amount,
            request.Description,
            request.ReceivedAt);

        incomeRepository.Update(income);
        await unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var income = await incomeRepository.GetByIdAsync(id);
        if (income == null)
            throw new KeyNotFoundException($"Income with id {id} not found.");

        var account = await accountRepository.GetByIdAsync(income.AccountId);
        if (account == null)
            throw new KeyNotFoundException($"Account with id {income.AccountId} not found.");

        account.Withdraw(income.Amount);
        await incomeRepository.DeleteAsync(id);
        await unitOfWork.CommitAsync();
    }

    private static IncomeResponse MapToResponse(Income income)
    {
        return new IncomeResponse(
            income.Id,
            income.AccountId,
            income.CategoryId,
            income.Amount,
            income.Description,
            income.ReceivedAt);
    }
}
