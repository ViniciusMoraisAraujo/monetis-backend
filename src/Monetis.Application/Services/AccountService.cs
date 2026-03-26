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
    public async Task<AccountDto?> GetByIdAsync(Guid id)
    {
        logger.LogInformation("Getting account by id: {Id}", id);
        var account= await accountRepository.GetByIdReadOnlyAsync(id);
        return account == null ? null : new AccountDto(account.Id, account.Name, account.UserId, account.Type, account.Balance, account.Currency);
    }

    public async Task<IEnumerable<AccountDto>> GetAllAsync()
    {
        logger.LogInformation("Getting all accounts");
        var accounts = await accountRepository.GetAllReadOnlyAsync();
        return accounts.Select(a => new AccountDto(a.Id, a.Name, a.UserId, a.Type, a.Balance, a.Currency));
    }

    public async Task<AccountDto> CreateAsync(CreateAccountDto createDto, Guid userId)
    {
        logger.LogInformation("Creating account: {Name}", createDto.Name);
        var account = new Account(createDto.Name, userId, createDto.Type, createDto.Currency);
        accountRepository.Create(account);
        await unitOfWork.CommitAsync();
        return new AccountDto(account.Id, account.Name, account.UserId, account.Type, account.Balance, account.Currency);
    }

    public async Task UpdateAsync(Guid id, UpdateAccountDto updateDto)
    {
        logger.LogInformation("Updating account: {Id}", id);
        var account = await accountRepository.GetByIdReadOnlyAsync(id);
        if (account == null)
            throw new KeyNotFoundException($"Account with id {id} not found.");

        account.Update(updateDto.Name);
        
        accountRepository.Update(account);
        await unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        logger.LogInformation("Deleting account: {Id}", id);
        await accountRepository.DeleteAsync(id);
        await unitOfWork.CommitAsync();
    }
}
