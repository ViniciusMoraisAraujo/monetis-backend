using Microsoft.Extensions.Logging;
using Monetis.Application.DTOs;
using Monetis.Application.Interfaces;
using Monetis.Domain.Entities.Transactions;
using Monetis.Domain.Interfaces;

namespace Monetis.Application.Services;

public class TransferService(
    ITransferRepository transferRepository,
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    ILogger<TransferService> logger)
    : ITransferService
{
    public async Task<TransferResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting transfer by id: {Id}", id);

        var transfer = await transferRepository.GetByIdReadOnlyAsync(id, cancellationToken);
        return transfer == null ? null : MapToDto(transfer);
    }

    public async Task<IEnumerable<TransferResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all transfers");

        var transfers = await transferRepository.GetAllReadOnlyAsync(cancellationToken);
        return transfers.Select(MapToDto);
    }

    public async Task<TransferResponse> CreateAsync(
        CreateTransferRequest createDto,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Creating transfer from account {OriginAccountId} to account {DestinationAccountId}",
            createDto.AccountId,
            createDto.DestinationAccountId);

        var originAccount = await accountRepository.GetByIdAsync(createDto.AccountId, cancellationToken);
        var destinationAccount = await accountRepository.GetByIdAsync(createDto.DestinationAccountId, cancellationToken);

        if (originAccount == null || destinationAccount == null)
            throw new KeyNotFoundException("Origin or destination account not found.");

        var transfer = new Transfer(
            originAccount,
            destinationAccount,
            createDto.Amount,
            createDto.Description,
            createDto.TransferredAt);

        transferRepository.Create(transfer);
        await unitOfWork.CommitAsync(cancellationToken);

        return MapToDto(transfer);
    }

    public async Task UpdateAsync(Guid id, UpdateTransferRequest updateDto, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating transfer: {Id}", id);

        var transfer = await transferRepository.GetByIdWithAccountsAsync(id, cancellationToken);
        if (transfer == null)
            throw new KeyNotFoundException($"Transfer with id {id} not found.");

        if (transfer.IsCancelled)
            throw new InvalidOperationException("Cancelled transfers cannot be updated.");

        var amountDelta = updateDto.Amount - transfer.Amount;
        if (amountDelta > 0)
            transfer.Account.Withdraw(amountDelta);
        else if (amountDelta < 0)
            transfer.Account.Deposit(Math.Abs(amountDelta));

        if (amountDelta > 0)
            transfer.DestinationAccount.Deposit(amountDelta);
        else if (amountDelta < 0)
            transfer.DestinationAccount.Withdraw(Math.Abs(amountDelta));

        transfer.Update(updateDto.Amount, updateDto.Description);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Cancelling transfer: {Id}", id);

        var transfer = await transferRepository.GetByIdWithAccountsAsync(id, cancellationToken);
        if (transfer == null)
            throw new KeyNotFoundException($"Transfer with id {id} not found.");

        transfer.Cancel(transfer.Account, DateTime.UtcNow);
        await unitOfWork.CommitAsync(cancellationToken);
    }

    private static TransferResponse MapToDto(Transfer transfer)
    {
        return new TransferResponse(
            transfer.Id,
            transfer.AccountId,
            transfer.DestinationAccountId,
            transfer.Amount,
            transfer.Description,
            transfer.TransferredAt);
    }
}
