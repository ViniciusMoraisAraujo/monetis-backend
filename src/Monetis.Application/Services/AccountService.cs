using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AccountDto?> GetByIdAsync(Guid id)
    {
        var account = await _accountRepository.GetByIdReadOnlyAsync(id);
        return account == null ? null : new AccountDto(account.Id, account.Name, account.UserId, account.Type, account.Balance, account.Currency);
    }

    public async Task<IEnumerable<AccountDto>> GetAllAsync()
    {
        var accounts = await _accountRepository.GetAllAsync();
        return accounts.Select(a => new AccountDto(a.Id, a.Name, a.UserId, a.Type, a.Balance, a.Currency));
    }

    public async Task<AccountDto> CreateAsync(CreateAccountDto createDto)
    {
        var account = new Account(createDto.Name, createDto.UserId, createDto.Type, createDto.Currency);
        await _accountRepository.Create(account);
        await _unitOfWork.CommitAsync();
        return new AccountDto(account.Id, account.Name, account.UserId, account.Type, account.Balance, account.Currency);
    }

    public async Task UpdateAsync(Guid id, UpdateAccountDto updateDto)
    {
        var account = await _accountRepository.GetByIdReadOnlyAsync(id);
        if (account == null)
            throw new KeyNotFoundException($"Account with id {id} not found.");

        account.Update(updateDto.Name);
        
        _accountRepository.Update(account);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _accountRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}