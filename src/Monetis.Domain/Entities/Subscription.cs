using Monetis.Domain.Entities.Transactions;
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
    public DateTime? EndDate { get; private set; }
    
    public bool IsActive { get; private set; } = true;
    public DateTime? LastProcessedAt { get; private set; }

    public PaymentMethod PaymentMethod { get; private set; }
    public Guid? CardId { get; private set; }
    public Card Card { get; private set; }
    
    
    protected Subscription() { }

    public Subscription(
        Guid userId, 
        Guid accountId,
        Guid categoryId, 
        decimal amount, 
        string description, 
        Frequency frequency, 
        DateTime nextDueDate,
        PaymentMethod paymentMethod,
        Guid? cardId = null,
        DateTime? endDate = null)
    {
        ValidateCreation(userId, categoryId, amount, description, paymentMethod, cardId, accountId);
        
        UserId = userId;
        CategoryId = categoryId;
        Amount = amount;
        Description = description;
        Frequency = frequency;
        NextDueDate = nextDueDate.Date; 
        PaymentMethod = paymentMethod;
        CardId = cardId;
        AccountId = accountId;
        EndDate = endDate;
        IsActive = true;
        LastProcessedAt = null;
    }

    public void Update(decimal amount,
        string description,
        Frequency frequency,
        DateTime nextDueDate,
        bool isActive,
        PaymentMethod paymentMethod,
        Guid accountId,
        DateTime? endDate = null,
        Guid? creditCardId = null)
    {
        if (!IsActive && isActive)
            Reactivate(); 
        
        ValidateUpdate(amount, description, paymentMethod, creditCardId, accountId);
        
        Amount = amount;
        Description = description;
        Frequency = frequency;
        NextDueDate = nextDueDate.Date;
        IsActive = isActive;
        PaymentMethod = paymentMethod;
        CardId = creditCardId;
        AccountId = accountId;
        EndDate = endDate;
    }
    
    public Expense Process(DateTime? processingDate = null)
    {
        if (!IsActive)
            throw new ArgumentException("Cannot process inactive subscription");
        
        if (EndDate.HasValue && NextDueDate > EndDate.Value)
            throw new ArgumentException("Subscription has reached end date");
        
        var processDate = processingDate ?? DateTime.UtcNow;
        
        if (NextDueDate.Date > processDate.Date)
            throw new ArgumentException($"Subscription is not due yet. Next due: {NextDueDate:dd/MM/yyyy}");
        
        var expense = new Expense(
            userId: UserId,
            accountId: AccountId,
            categoryId: CategoryId,
            amount: Amount,
            description: $"{Description} (Assinatura)",
            dueDate: NextDueDate,
            paymentMethod: PaymentMethod,
            creditCardId: CardId
        );

        CalculateNextDueDate();
        LastProcessedAt = processDate;
        
        if (EndDate.HasValue && NextDueDate > EndDate.Value)
            IsActive = false;

        return expense;
    }

    public void Cancel()
    {
        IsActive = false;
    }

    private void Reactivate(DateTime? newNextDueDate = null)
    {
        if (EndDate.HasValue && DateTime.UtcNow > EndDate.Value)
            throw new ArgumentException("Cannot reactivate subscription after end date");
        
        IsActive = true;
        
        if (newNextDueDate.HasValue)
            NextDueDate = newNextDueDate.Value;
        else if (NextDueDate < DateTime.UtcNow.Date)
            CalculateNextDueDate(fromDate: DateTime.UtcNow);
    }

    private void CalculateNextDueDate(DateTime? fromDate = null)
    {
        var baseDate = fromDate ?? NextDueDate;
        
        NextDueDate = Frequency switch
        {
            Frequency.Weekly => baseDate.AddDays(7),
            Frequency.Biweekly => baseDate.AddDays(15),
            Frequency.Monthly => baseDate.AddMonths(1),
            Frequency.Bimonthly => baseDate.AddMonths(2),
            Frequency.Quarterly => baseDate.AddMonths(3),
            Frequency.Semiannual => baseDate.AddMonths(6),
            Frequency.Yearly => baseDate.AddYears(1),
            _ => throw new ArgumentException("Invalid frequency")
        };
    }

    private void ValidateCreation(Guid userId, Guid categoryId, decimal amount, 
        string description, PaymentMethod paymentMethod, Guid? cardId, Guid? accountId)
    {
        if (userId == Guid.Empty) 
            throw new ArgumentException("User is required");
        
        if (categoryId == Guid.Empty) 
            throw new ArgumentException("Category is required");
        
        if (amount <= 0) 
            throw new ArgumentException("Amount must be greater than zero");
        
        if (string.IsNullOrWhiteSpace(description) || description.Length > 200)
            throw new ArgumentException("Description is required and must be less than 200 characters");

        ValidatePaymentMethod(paymentMethod, cardId, accountId);
    }

    private void ValidateUpdate(decimal amount, string description, 
        PaymentMethod paymentMethod, Guid? cardId, Guid? accountId)
    {
        if (amount <= 0) 
            throw new ArgumentException("Amount must be greater than zero");
        
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required");

        ValidatePaymentMethod(paymentMethod, cardId, accountId);
    }

    private void ValidatePaymentMethod(PaymentMethod paymentMethod, Guid? cardId, Guid? accountId)
    {
        if (paymentMethod == PaymentMethod.CreditCard)
        {
            if (!cardId.HasValue || cardId.Value == Guid.Empty)
                throw new ArgumentException("Card is required for credit card payments");
            
            if (!accountId.HasValue || accountId.Value == Guid.Empty)
                throw new ArgumentException($"Account is required for {paymentMethod} payments");

        }
        else
        {
            if (!accountId.HasValue || accountId.Value == Guid.Empty)
                throw new ArgumentException($"Account is required for {paymentMethod} payments");

            if (cardId.HasValue)
                throw new ArgumentException("Card should only be specified for credit card payments");
        }
    }
}