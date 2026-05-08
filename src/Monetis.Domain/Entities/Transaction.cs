using Monetis.Domain.Exceptions;

namespace Monetis.Domain.Entities;
public abstract class Transaction : UserOwnedEntity
{
    public Guid AccountId { get; private set; }
    public Account Account { get; private set; }

    public decimal Amount { get; private set; }
    public string Description { get; private set; }

    protected Transaction() { }

    protected Transaction(Guid accountId, decimal amount, string description)
    {
        ChangeAccount(accountId);
        UpdateBase(amount, description);
    }

    protected void UpdateBase(decimal amount, string description)
    {
        ValidateAmount(amount);
        ValidateDescription(description);

        Amount = amount;
        Description = description;
    }

    protected void ChangeAccount(Guid newAccountId)
    {
        if (newAccountId == Guid.Empty)
            throw new TransactionInvalidAccountException();

        AccountId = newAccountId;
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new TransactionAmountMustBePositiveException();
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new TransactionDescriptionRequiredException();

        if (description.Length > 200)
            throw new TransactionDescriptionTooLongException();
    }
}
