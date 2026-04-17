using Monetis.Domain.Enums;

namespace Monetis.Domain.Entities.Transactions;

public class Expense : Transaction
{
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; }
    public DateTime DueDate { get; private set; }
    public TransactionStatus Status { get; private set; }
    public DateTime? PaidAt { get; private set; }

    //logic of installment
    public bool IsInstallment { get; private set; }
    public int? InstallmentNumber { get; private set; }
    public int? TotalInstallments { get; private set; }
    public Guid? InstallmentGroupId { get; private set; }
    
    //logic of card
    public PaymentMethod PaymentMethod { get; private set; }
    public Guid? CreditCardId { get; private set; }
    public bool IsPaidInCash => PaymentMethod == PaymentMethod.Cash || PaymentMethod == PaymentMethod.Debit || PaymentMethod == PaymentMethod.Pix;
    
    
    protected Expense() { }
    
    public Expense(Guid userId, Guid accountId, Guid categoryId,
        decimal amount, string description, DateTime dueDate,
        PaymentMethod paymentMethod = PaymentMethod.Cash, Guid? creditCardId = null)
        : base(userId, accountId, amount, description)
    {
        ValidateExpense(categoryId, dueDate, paymentMethod, creditCardId);
        
        CategoryId = categoryId;
        DueDate = dueDate;
        Status = TransactionStatus.Pending;
        PaymentMethod = paymentMethod;
        CreditCardId = creditCardId;
        IsInstallment = false;
    }
    private Expense(Guid userId, Guid accountId, Guid categoryId,
        decimal amount, string description, DateTime dueDate,
        PaymentMethod paymentMethod, Guid? creditCardId,
        bool isInstallment, int? installmentNumber, int? totalInstallments, 
        Guid installmentGroupId = new Guid())
        : base(userId, accountId, amount, description)
    {
        CategoryId = categoryId;
        DueDate = dueDate;
        Status = TransactionStatus.Pending;
        PaymentMethod = paymentMethod;
        CreditCardId = creditCardId;
        IsInstallment = isInstallment;
        InstallmentNumber = installmentNumber;
        TotalInstallments = totalInstallments;
        InstallmentGroupId = installmentGroupId;
    }

    public static IReadOnlyCollection<Expense> CreateInstallment(
        Guid userId, Guid accountId, Guid categoryId,
        decimal totalAmount, string description, DateTime firstDueData,
        int numberOfInstallments, PaymentMethod paymentMethod, Guid creditCardId )
    {
        ValidateInstallmentParameters(totalAmount, numberOfInstallments, creditCardId);
        
        var installmentAmount = Math.Round(totalAmount / numberOfInstallments, 2);
        var adjustment = totalAmount - (installmentAmount * numberOfInstallments);
        var groupId = Guid.NewGuid();
        var expenses = new List<Expense>(numberOfInstallments);

        for (int i = 0; i < numberOfInstallments; i++)
        {
            var amount = (i == 0) ? installmentAmount + adjustment : installmentAmount;
            var dueDate = firstDueData.AddMonths(i);
            var installmentDescription = $"{description} ({i + 1}/{numberOfInstallments})";

            var expense = new Expense(
                userId: userId,
                accountId: accountId,
                categoryId: categoryId,
                amount: amount,
                description: installmentDescription,
                dueDate: dueDate,
                paymentMethod: paymentMethod,
                creditCardId: creditCardId,
                isInstallment: true,
                installmentNumber: i + 1,
                totalInstallments: numberOfInstallments,
                installmentGroupId: groupId
            );
            
            expenses.Add(expense);
        }

        return expenses.AsReadOnly();
    }
    
    public void Pay(DateTime paidAt, Guid actualAccountId)
    {
        if (Status == TransactionStatus.Paid)
            throw new InvalidOperationException("This expense is already paid.");

        if (AccountId != actualAccountId)
        {
            this.ChangeAccount(actualAccountId);
        }

        PaidAt = paidAt;
        Status = TransactionStatus.Paid;
    }

    public void MarkAsOverDue()
    {
        if (Status == TransactionStatus.Pending && DueDate.Date < DateTime.UtcNow.Date)
            Status = TransactionStatus.Overdue;
    }
    
    
    public void Update(Guid categoryId, decimal amount, string description, DateTime dueDate)
    {
        if (Status == TransactionStatus.Paid)
            throw new ArgumentException("Cannot update a paid expense");
            
        if (IsInstallment)
            throw new ArgumentException("Cannot update individual installment. Update the entire group.");

        if (categoryId == Guid.Empty) 
            throw new ArgumentException("Category is required");

        
        CategoryId = categoryId;
        UpdateBase(amount, description);
        DueDate = dueDate;
    }
    
    private static void ValidateInstallmentParameters(decimal totalAmount, int numberOfInstallments, Guid creditCardId)
    {
        if (numberOfInstallments < 2 || numberOfInstallments > 24)
            throw new ArgumentException("Installments must be between 2 and 24");
        
        if (totalAmount <= 0)
            throw new ArgumentException("Total amount must be greater than zero");

        if (creditCardId == Guid.Empty)
            throw new ArgumentException("Credit card is required for installments");
    }
    

    private void ValidateExpense(Guid categoryId, DateTime dueDate,
        PaymentMethod paymentMethod, Guid? creditCardId)
    {
        if (categoryId == Guid.Empty) 
            throw new ArgumentException("Category is required");
        
        if (dueDate < DateTime.UtcNow.Date.AddYears(-1))
            throw new ArgumentException("Due date is too far in the past");
        
        if (paymentMethod == PaymentMethod.CreditCard && !creditCardId.HasValue)
            throw new ArgumentException("Credit card is required for credit card payments");
            
        if (paymentMethod != PaymentMethod.CreditCard && creditCardId.HasValue)
            throw new ArgumentException("Credit card should only be set for credit card payments");
    }
}