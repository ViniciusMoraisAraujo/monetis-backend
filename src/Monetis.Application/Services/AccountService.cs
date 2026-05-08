using Microsoft.Extensions.Logging;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class AccountService(
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    ILogger<AccountService> logger)
    : IAccountService
{
    public async Task<AccountResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting account by id: {Id}", id);
        var account= await accountRepository.GetByIdReadOnlyAsync(id, cancellationToken);
        return account == null ? null : new AccountResponse(account.Id, account.Name, account.UserId, account.Type, account.Balance);
    }

    public async Task<IEnumerable<AccountResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all accounts");
        var accounts = await accountRepository.GetAllReadOnlyAsync(cancellationToken);
        return accounts.Select(a => new AccountResponse(a.Id, a.Name, a.UserId, a.Type, a.Balance));
    }

    public async Task<AccountResponse> CreateAsync(CreateAccountRequest createDto, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating account: {Name}", createDto.Name);
        var account = new Account(createDto.Name, createDto.Type);
        accountRepository.Create(account);
        await unitOfWork.CommitAsync(cancellationToken);
        return new AccountResponse(account.Id, account.Name, account.UserId, account.Type, account.Balance);
    }

    public async Task UpdateAsync(Guid id, UpdateAccountRequest updateDto, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating account: {Id}", id);
        var account = await accountRepository.GetByIdAsync(id, cancellationToken);
        if (account == null)
            throw new KeyNotFoundException($"Account with id {id} not found.");

        account.Update(updateDto.Name);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting account: {Id}", id);
        await accountRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
