using Monetis.Domain.Enums;

namespace Monetis.Domain.Entities;

public class Subscription : BaseEntity
{
    public User User { get; private set; }
    public Guid UserId { get; private set; } 
    
    public Account Account { get; private set; }
    public Guid AccountId { get; private set; }
    
    public Category Category { get; private set; }
    public Guid CategoryId { get; private set; }
    
    public decimal Amount { get; private set; }
    public string Description { get; private set; }
    public Frequency Frequency { get; private set; }
    public DateTime NextDueDate { get; private set; }
    public bool IsActive { get; private set; } = true;

    protected Subscription() { }

    public Subscription(Guid userId, Guid accountId, Guid categoryId, decimal amount, string description, Frequency frequency, DateTime nextDueDate, bool isActive)
    {
        UserId = userId;
        AccountId = accountId;
        CategoryId = categoryId;
        Amount = amount;
        Description = description;
        Frequency = frequency;
        NextDueDate = nextDueDate;
        IsActive = isActive;
    }

    public void Update(decimal amount, string description, Frequency frequency, DateTime nextDueDate, bool isActive)
    {
        Amount = amount;
        Description = description;
        Frequency = frequency;
        NextDueDate = nextDueDate;
        IsActive = isActive;
    }
}