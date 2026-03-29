using Monetis.Domain.Enums;

namespace Monetis.Domain.Entities.Transactions;

public class Expense : Transaction
{
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; }
    
    public DateTime DueDate { get; private set; }
    public TransactionStatus Status { get; private set; }
    public DateTime? PaidAt { get; private set; }
    
    protected Expense() { }
    
    public Expense(Guid userId, Guid accountId, Guid categoryId,
        decimal amount, string description, DateTime dueDate)
        : base(userId, accountId, amount, description)
    {
        DueDate = dueDate;
        Status = TransactionStatus.Pending;
    } 
    
    public void MarkAsPaid(DateTime paidAt)
    {
        if(Status == TransactionStatus.Paid)
            throw new InvalidOperationException("Expense is already paid");
        
        PaidAt = paidAt;
        Status = TransactionStatus.Paid;
    }
    
    public void Update(Guid categoryId, decimal amount, string description, DateTime dueDate)
    {
        CategoryId = categoryId;
        UpdateBase(amount, description);
        DueDate = dueDate;
    }
}