using Monetis.Application.Abstractions.Persistence;
using Monetis.Application.Abstractions.Services;
using Monetis.Application.DTOs;
using Monetis.Domain.Entities.Transactions;

namespace Monetis.Application.Services;

public class IncomeService(
    IIncomeRepository incomeRepository,
    IUnitOfWork unitOfWork,
    IUserResourceGuard userResourceGuard)
    : IIncomeService
{
    public async Task<IncomeResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var income = await incomeRepository.GetByIdReadOnlyAsync(id, cancellationToken);
        return income == null ? null : MapToResponse(income);
    }

    public async Task<IEnumerable<IncomeResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var incomes = await incomeRepository.GetAllReadOnlyAsync(cancellationToken);
        return incomes.Select(MapToResponse);
    }

    public async Task<IncomeResponse> CreateAsync(CreateIncomeRequest request, CancellationToken cancellationToken = default)
    {
        var account = await userResourceGuard.GetOwnedAccountAsync(request.AccountId, cancellationToken);
        _ = await userResourceGuard.GetVisibleCategoryAsync(request.CategoryId, cancellationToken);

        var income = Income.CreatePaid(
            request.AccountId,
            request.CategoryId,
            request.Amount,
            request.Description,
            request.ReceivedAt);

        account.Deposit(income.Amount);
        incomeRepository.Create(income);
        await unitOfWork.CommitAsync(cancellationToken);

        return MapToResponse(income);
    }

    public async Task UpdateAsync(Guid id, UpdateIncomeRequest request, CancellationToken cancellationToken = default)
    {
        var income = await incomeRepository.GetByIdAsync(id, cancellationToken);
        if (income == null)
            throw new KeyNotFoundException($"Income with id {id} not found.");

        var account = await userResourceGuard.GetOwnedAccountAsync(income.AccountId, cancellationToken);
        _ = await userResourceGuard.GetVisibleCategoryAsync(request.CategoryId, cancellationToken);

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
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var income = await incomeRepository.GetByIdAsync(id, cancellationToken);
        if (income == null)
            throw new KeyNotFoundException($"Income with id {id} not found.");

        var account = await userResourceGuard.GetOwnedAccountAsync(income.AccountId, cancellationToken);

        account.Withdraw(income.Amount);
        await incomeRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
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
