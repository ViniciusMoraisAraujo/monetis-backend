using Microsoft.Extensions.Logging;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities;
using Monetis.Domain.Enums;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class TransactionService(
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork,
    ILogger<TransactionService> logger)
    : ITransactionService
{
    public async Task<TransactionDto?> GetByIdAsync(Guid id)
    {
        logger.LogInformation("Getting transaction by id: {Id}", id);
        var transaction = await transactionRepository.GetByIdReadOnlyAsync(id);
        return transaction == null ? null : new TransactionDto(transaction.Id, transaction.Amount, transaction.Type, 
            transaction.DueDate, transaction.PaidAt, transaction.Description, transaction.Status ?? TransactionStatus.Pending);
    }

    public async Task<IEnumerable<TransactionDto>> GetAllAsync()
    {
        logger.LogInformation("Getting all transactions");
        var transactions = await transactionRepository.GetAllReadOnlyAsync();
        return transactions.Select(transaction => new TransactionDto(transaction.Id, transaction.Amount, transaction.Type, 
            transaction.DueDate, transaction.PaidAt, transaction.Description, transaction.Status ?? TransactionStatus.Pending));
    }
    
    public async Task<TransactionDto> CreateAsync(CreateTransactionDto createDto, Guid userId)
    {
        logger.LogInformation("Creating transaction: {Description}", createDto.Description);
        var transaction = new Transaction(userId, createDto.AccountId, createDto.CategoryId, createDto.Amount, createDto.Type, createDto.PaidAt, createDto.Description);
        transactionRepository.Create(transaction);
        unitOfWork.CommitAsync();
        return new TransactionDto(transaction.Id, transaction.Amount, transaction.Type, 
            transaction.DueDate, transaction.PaidAt, transaction.Description, transaction.Status ?? TransactionStatus.Pending);
    }

    public async Task UpdateAsync(Guid id, UpdateTransactionDto updateDto)
    {
        logger.LogInformation("Updating transaction: {Id}", id);
        var transaction = await transactionRepository.GetByIdReadOnlyAsync(id);
        if (transaction == null)
            throw new KeyNotFoundException($"Transaction with id {id} not found.");

        transaction.Update(updateDto.CategoryId, updateDto.Amount, updateDto.Description);
        
        transactionRepository.Update(transaction);
        await unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        logger.LogInformation("Deleting transaction: {Id}", id);
        await transactionRepository.DeleteAsync(id);
        await unitOfWork.CommitAsync();
    }
}
