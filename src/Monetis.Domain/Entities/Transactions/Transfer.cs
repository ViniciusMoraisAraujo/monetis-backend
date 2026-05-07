using Monetis.Domain.Exceptions;

namespace Monetis.Domain.Entities.Transactions;

public class Transfer : Transaction
{
    public Guid DestinationAccountId { get; private set; }

    public Account DestinationAccount { get; private set; }

    public DateTime TransferredAt { get; private set; }

    public bool IsCancelled { get; private set; }

    protected Transfer()
    {
    }

    public Transfer(
        Account originAccount,
        Account destinationAccount,
        decimal amount,
        string description,
        DateTime transferredAt)
        : base(originAccount.Id, amount, description)
    {
        ValidateCreation(originAccount, destinationAccount, amount, transferredAt);
        ValidateSufficientFunds(originAccount, amount);
        TransferAmount(originAccount, destinationAccount, amount);
        DestinationAccountId = destinationAccount.Id;
        DestinationAccount = destinationAccount;
        TransferredAt = transferredAt;
        IsCancelled = false;
    }
    
    private void ValidateCreation(Account originAccount, Account destinationAccount,
        decimal amount, DateTime transferredAt)
    {
        if (originAccount == null)
            throw new TransferOriginAccountRequiredException();

        if (destinationAccount == null)
            throw new TransferDestinationAccountRequiredException();

        if (originAccount.Id == destinationAccount.Id)
            throw new TransferAccountsMustBeDifferentException();

        if (originAccount.UserId != destinationAccount.UserId)
            throw new TransferAccountsMustBelongToSameUserException();
        
        if (amount <= 0)
            throw new TransferAmountMustBePositiveException();

        if (transferredAt > DateTime.UtcNow.AddMinutes(5))
            throw new TransferDateInFutureException();
    }

    private void ValidateSufficientFunds(Account originAccount, decimal amount)
    {
        if (originAccount.Balance < amount)
            throw new TransferInsufficientFundsException(originAccount.Balance, amount);
    }

    public void Cancel(Account originAccount,DateTime cancellationDate)
    {
        if (IsCancelled)
            throw new TransferAlreadyCancelledException();

        if ((cancellationDate.Date - TransferredAt.Date).Days != 0)
            throw new TransferCancellationSameDayOnlyException();
        
        if (originAccount.Id != AccountId)
            throw new TransferOriginAccountMismatchException();
        
        TransferAmount(DestinationAccount, originAccount, Amount);
        IsCancelled = true;
    }

    private void TransferAmount(Account originAccount, Account destinationAccount, decimal amount)
    {
        originAccount.Withdraw(amount);
        destinationAccount.Deposit(amount);
    }
}
