using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TransactionDto?> GetByIdAsync(Guid id)
    {
        var transaction = await _transactionRepository.GetByIdReadOnlyAsync(id);
        return transaction == null ? null : new TransactionDto(transaction.Id, transaction.UserId, transaction.AccountId, transaction.CategoryId, transaction.Amount, transaction.Type, transaction.PaidAt, transaction.Description);
    }

    public async Task<IEnumerable<TransactionDto>> GetAllAsync()
    {
        var transactions = await _transactionRepository.GetAllAsync();
        return transactions.Select(t => new TransactionDto(t.Id, t.UserId, t.AccountId, t.CategoryId, t.Amount, t.Type, t.PaidAt, t.Description));
    }

    public async Task<TransactionDto> CreateAsync(CreateTransactionDto createDto)
    {
        var transaction = new Transaction(createDto.UserId, createDto.AccountId, createDto.CategoryId, createDto.Amount, createDto.Type, createDto.PaidAt, createDto.Description);
        await _transactionRepository.Create(transaction);
        await _unitOfWork.CommitAsync();
        return new TransactionDto(transaction.Id, transaction.UserId, transaction.AccountId, transaction.CategoryId, transaction.Amount, transaction.Type, transaction.PaidAt, transaction.Description);
    }

    public async Task UpdateAsync(Guid id, UpdateTransactionDto updateDto)
    {
        var transaction = await _transactionRepository.GetByIdReadOnlyAsync(id);
        if (transaction == null)
            throw new KeyNotFoundException($"Transaction with id {id} not found.");

        transaction.Update(updateDto.CategoryId, updateDto.Amount, updateDto.Description);
        
        _transactionRepository.Update(transaction);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _transactionRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}