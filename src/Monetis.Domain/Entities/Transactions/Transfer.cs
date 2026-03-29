namespace Monetis.Domain.Entities.Transactions;

public class Transfer : Transaction
{
    public Guid DestinationAccountId { get; private set; }
    public Account DestinationAccount { get; private set; }
    public DateTime TransferredAt { get; private set; }

    protected Transfer() { }

    public Transfer(Guid userId, Guid accountId, Guid destinationAccountId,
        decimal amount, string description, DateTime transferredAt)
        : base(userId, accountId, amount, description) 
    {
        DestinationAccountId = destinationAccountId;
        TransferredAt = transferredAt;
    }
}