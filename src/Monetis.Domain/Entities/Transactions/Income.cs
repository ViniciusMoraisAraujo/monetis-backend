using Monetis.Domain.Enums;
using Monetis.Domain.Exceptions;

namespace Monetis.Domain.Entities.Transactions;

public class Income : Transaction
{
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; }
    public DateTime ReceivedAt { get; private set; }
    public TransactionStatus Status { get; private set; }

    protected Income() { }

    private Income(Guid accountId, Guid categoryId,
        decimal amount, string description, DateTime date, TransactionStatus status)
        : base(accountId, amount, description)
    {
        ValidateIncome(categoryId, amount);

        CategoryId = categoryId;
        ReceivedAt = date;
        Status = status;
    }

    public static Income CreatePaid(Guid accountId, Guid categoryId,
        decimal amount, string description, DateTime receivedAt)
    {
        if (receivedAt.Date > DateTime.UtcNow.Date)
            throw new IncomeReceivedDateInFutureException();

        return new Income(accountId, categoryId, amount, description, receivedAt, TransactionStatus.Paid);
    }

    public static Income Schedule(Guid accountId, Guid categoryId,
        decimal amount, string description, DateTime expectedDate)
    {
        if (expectedDate.Date <= DateTime.UtcNow.Date)
            throw new IncomeExpectedDateMustBeFutureException();

        return new Income(accountId, categoryId, amount, description, expectedDate, TransactionStatus.Pending);
    }

    public void Update(Guid categoryId, decimal amount, string description, DateTime receivedAt)
    {
        ValidateIncome(categoryId, amount);
        
        if (receivedAt.Date > DateTime.UtcNow.Date && Status == TransactionStatus.Paid)
            throw new IncomeReceivedDateInFutureException();

        CategoryId = categoryId;
        ReceivedAt = receivedAt;
        UpdateBase(amount, description);
    }

    public void ConfirmReceipt(DateTime? actualDate = null)
    {
        if (Status == TransactionStatus.Paid)
            throw new IncomeAlreadyReceivedException();

        ReceivedAt = actualDate ?? DateTime.UtcNow;
        Status = TransactionStatus.Paid;
    }

    public void Cancel()
    {
        if (Status == TransactionStatus.Paid)
            throw new ReceivedIncomeCannotBeCancelledException();
        
        Status = TransactionStatus.Cancelled;
    }

    private void ValidateIncome(Guid categoryId, decimal amount)
    {
        if (amount <= 0)
            throw new IncomeAmountMustBePositiveException();
        
        if (categoryId == Guid.Empty)
            throw new IncomeCategoryRequiredException();
    }
}
