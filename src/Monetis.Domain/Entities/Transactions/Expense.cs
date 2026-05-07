using Monetis.Domain.Enums;
using Monetis.Domain.Exceptions;

namespace Monetis.Domain.Entities.Transactions;

public class Expense : Transaction
{
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; }
    public DateTime DueDate { get; private set; }
    public TransactionStatus Status { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public Guid? SubscriptionId { get; private set; }
    public Subscription Subscription { get; private set; }

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
    
    public Expense( Guid accountId, Guid categoryId,
        decimal amount, string description, DateTime dueDate,
        PaymentMethod paymentMethod = PaymentMethod.Cash, Guid? creditCardId = null, Guid? subscriptionId = null)
        : base( accountId, amount, description)
    {
        ValidateExpense(categoryId, dueDate, paymentMethod, creditCardId);
        
        CategoryId = categoryId;
        DueDate = dueDate;
        Status = TransactionStatus.Pending;
        PaymentMethod = paymentMethod;
        CreditCardId = creditCardId;
        IsInstallment = false;
        SubscriptionId = subscriptionId;
    }
    private Expense(Guid accountId, Guid categoryId,
        decimal amount, string description, DateTime dueDate,
        PaymentMethod paymentMethod, Guid? creditCardId,
        bool isInstallment, int? installmentNumber, int? totalInstallments,
        Guid? subscriptionId = null,
        Guid installmentGroupId = new Guid())
        : base( accountId, amount, description)
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
        SubscriptionId = subscriptionId;
    }

    public static IReadOnlyCollection<Expense> CreateInstallment(
        Guid accountId, Guid categoryId,
        decimal totalAmount, string description, DateTime firstDueData,
        int numberOfInstallments, Guid creditCardId, Guid? subscriptionId = null )
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
                accountId: accountId,
                categoryId: categoryId,
                amount: amount,
                description: installmentDescription,
                dueDate: dueDate,
                paymentMethod: PaymentMethod.CreditCard,
                creditCardId: creditCardId,
                isInstallment: true,
                installmentNumber: i + 1,
                totalInstallments: numberOfInstallments,
                installmentGroupId: groupId,
                subscriptionId : subscriptionId
            );
            
            expenses.Add(expense);
        }

        return expenses.AsReadOnly();
    }
    
    public void Pay(DateTime paidAt, Guid actualAccountId)
    {
        if (Status == TransactionStatus.Paid)
            throw new ExpenseAlreadyPaidException();

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
            throw new PaidExpenseCannotBeUpdatedException();
            
        if (IsInstallment)
            throw new InstallmentExpenseCannotBeUpdatedException();

        if (categoryId == Guid.Empty) 
            throw new ExpenseCategoryRequiredException();

        
        CategoryId = categoryId;
        UpdateBase(amount, description);
        DueDate = dueDate;
    }
    
    private static void ValidateInstallmentParameters(decimal totalAmount, int numberOfInstallments, Guid creditCardId)
    {
        if (numberOfInstallments < 2 || numberOfInstallments > 24)
            throw new ExpenseInstallmentRangeException();
        
        if (totalAmount <= 0)
            throw new ExpenseTotalAmountMustBePositiveException();

        if (creditCardId == Guid.Empty)
            throw new ExpenseCreditCardRequiredForInstallmentsException();
    }
    

    private void ValidateExpense(Guid categoryId, DateTime dueDate,
        PaymentMethod paymentMethod, Guid? creditCardId)
    {
        if (categoryId == Guid.Empty) 
            throw new ExpenseCategoryRequiredException();
        
        if (dueDate < DateTime.UtcNow.Date.AddYears(-1))
            throw new ExpenseDueDateTooOldException();
        
        if (paymentMethod == PaymentMethod.CreditCard && !creditCardId.HasValue)
            throw new ExpenseCreditCardRequiredException();
            
        if (paymentMethod != PaymentMethod.CreditCard && creditCardId.HasValue)
            throw new ExpenseCreditCardOnlyForCreditCardPaymentException();
    }
}
