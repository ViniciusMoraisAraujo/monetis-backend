using Monetis.Domain.Enums;

namespace Monetis.Domain.Entities;

public class Transaction : BaseEntity
{
    public User User { get; private set; }
    public User UserId { get; private set; }

    public Account Account { get; private set; }
    public Account AccountId { get; private set; }

    public Category Category { get; private set; }
    public Category CategoryId { get; private set; }
    
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public DateTime Date { get; private set; }
    public string Description { get; private set; }
    
    protected Transaction() { }

    public Transaction(User userId, Account accountId, Category categoryId, decimal amount, TransactionType type, DateTime date, string description)
    {
        UserId = userId;
        AccountId = accountId;
        CategoryId = categoryId;
        Amount = amount;
        Type = type;
        Date = date;
        Description = description;
    }
}