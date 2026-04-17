namespace Monetis.Domain.Entities;
public abstract class Transaction : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public Guid AccountId { get; private set; }
    public Account Account { get; private set; }

    public decimal Amount { get; private set; }
    public string Description { get; private set; }

    protected Transaction() { }

    protected Transaction(Guid userId, Guid accountId, decimal amount, string description)
    {
        UserId = userId;
        AccountId = accountId;
        Amount = amount;
        Description = description;
    }

    public void UpdateBase(decimal amount, string description)
    {
        Amount = amount;
        Description = description;
    }
    protected void ChangeAccount(Guid newAccountId)
    {
        if (newAccountId == Guid.Empty)
            throw new ArgumentException("Invalid Account Id");

        AccountId = newAccountId;
    }
}