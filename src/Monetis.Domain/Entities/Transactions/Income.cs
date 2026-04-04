using Monetis.Domain.Enums;

namespace Monetis.Domain.Entities.Transactions;

public class Income : Transaction
{
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; }
    public DateTime ReceivedAt { get; private set; }
    public TransactionStatus Status { get; private set; }

    protected Income() { }

    private Income(Guid userId, Guid accountId, Guid categoryId,
        decimal amount, string description, DateTime date, TransactionStatus status)
        : base(userId, accountId, amount, description)
    {
        ValidadteIncome(categoryId, amount);

        CategoryId = categoryId;
        ReceivedAt = date;
        Status = status;
    }

    public static Income CreatePaid(Guid userId, Guid accountId, Guid categoryId,
        decimal amount, string description, DateTime receivedAt)
    {
        if (receivedAt.Date > DateTime.UtcNow.Date)
            throw new ArgumentException("Received date cannot be in the future for paid incomes");

        return new Income(userId, accountId, categoryId, amount, description, receivedAt, TransactionStatus.Paid);
    }

    public static Income Schedule(Guid userId, Guid accountId, Guid categoryId,
        decimal amount, string description, DateTime expectedDate)
    {
        if (expectedDate.Date <= DateTime.UtcNow.Date)
            throw new ArgumentException("Expected date must be in the future for scheduled incomes");

        return new Income(userId, accountId, categoryId, amount, description, expectedDate, TransactionStatus.Pending);
    }

    public void Update(Guid categoryId, decimal amount, string description, DateTime receivedAt)
    {
        ValidadteIncome(categoryId, amount);
        
        if (receivedAt.Date > DateTime.UtcNow.Date && Status == TransactionStatus.Paid)
            throw new ArgumentException("Received date cannot be in the future for paid incomes");

        CategoryId = categoryId;
        ReceivedAt = receivedAt;
        UpdateBase(amount, description);
    }

    public void ConfirmReceipt(DateTime? actualDate = null)
    {
        if (Status == TransactionStatus.Paid)
            throw new ArgumentException("Income is already received");

        ReceivedAt = actualDate ?? DateTime.UtcNow;
        Status = TransactionStatus.Paid;
    }

    public void Cancel()
    {
        if (Status == TransactionStatus.Paid)
            throw new ArgumentException("Cannot cancel an already received income");
        
        Status = TransactionStatus.Cancelled;
    }

    private void ValidadteIncome(Guid categoryId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Income amount must be greater than zero");
        
        if (categoryId == Guid.Empty)
            throw new ArgumentException("Category is required");
    }
}