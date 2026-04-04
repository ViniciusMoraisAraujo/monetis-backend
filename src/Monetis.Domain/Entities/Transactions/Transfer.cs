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
        Guid userId,
        Account originAccount,
        Account destinationAccount,
        decimal amount,
        string description,
        DateTime transferredAt)
        : base(userId, originAccount.Id, amount, description)
    {
        try
        {
            ValidateCreation(originAccount, destinationAccount, amount, transferredAt);
            ValidateSufficientFunds(originAccount, amount);
            DestinationAccountId = destinationAccount.Id;
            DestinationAccount = destinationAccount;
            TransferredAt = transferredAt;
            IsCancelled = false;
            TransferAmount(originAccount, destinationAccount, amount);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private void ValidateCreation(Account originAccount, Account destinationAccount,
        decimal amount, DateTime transferredAt)
    {
        if (originAccount == null)
            throw new ArgumentException("Origin account is required");

        if (destinationAccount == null)
            throw new ArgumentException("Destination account is required");

        if (originAccount.Id == destinationAccount.Id)
            throw new ArgumentException("Origin and destination accounts must be different");

        if (originAccount.UserId != UserId || destinationAccount.UserId != UserId)
            throw new ArgumentException("Both accounts must belong to the same user");
        
        if (amount <= 0)
            throw new ArgumentException("Transfer amount must be greater than zero");

        if (transferredAt > DateTime.UtcNow.AddMinutes(5))
            throw new ArgumentException("Transfer date cannot be in the future");
    }

    private void ValidateSufficientFunds(Account originAccount, decimal amount)
    {
        if (originAccount.Balance < amount)
            throw new ArgumentException(
                $"Insufficient funds for transfer. Current balance: {originAccount.Balance:C}, " +
                $"Transfer amount: {amount:C}, " +
                $"Missing: {amount - originAccount.Balance:C}");
    }

    public void Cancel(Account originAccount,DateTime cancellationDate)
    {
        if (IsCancelled)
            throw new ArgumentException("Transfer is already cancelled");

        if ((cancellationDate.Date - TransferredAt.Date).Days != 0)
            throw new ArgumentException("Transfer can only be cancelled on the same day");
        
        if (originAccount.Id != AccountId)
            throw new ArgumentException("Origin account does not match the transfer's origin account");
        
        try
        {
            TransferAmount(DestinationAccount, originAccount, Amount);
            IsCancelled = true;
        }   
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void TransferAmount(Account originAccount, Account destinationAccount, decimal amount)
    {
        try
        {
            originAccount.Withdraw(amount);
            destinationAccount.Deposit(amount);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}