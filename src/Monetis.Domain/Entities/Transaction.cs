using Monetis.Domain.Enums;

namespace Monetis.Domain.Entities;
//"Se Transaction ganhar lógica de parcelamento ou recorrência, extrair para entidades separadas InstallmentPlan e RecurringRule com FK para Transaction.
public class Transaction : BaseEntity
{
    public User User { get; private set; }
    public Guid UserId { get; private set; }

    public Account Account { get; private set; }
    public Guid AccountId { get; private set; }

    public Category Category { get; private set; }
    public Guid CategoryId { get; private set; }
    
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public DateTime PaidAt { get; private set; }
    public string Description { get; private set; }
    public TransactionStatus? Status { get; private set; }
    public DateTime? DueDate { get; private set; }
    
    protected Transaction() { }

    public Transaction(Guid userId, Guid accountId, Guid categoryId, decimal amount, TransactionType type, DateTime paidAt, string description)
    {
        UserId = userId;
        AccountId = accountId;
        CategoryId = categoryId;
        Amount = amount;
        Type = type;
        PaidAt = paidAt;
        Description = description;
        if (Type == TransactionType.Expense)
        {
            Status = TransactionStatus.Pending;
        }
    }
}