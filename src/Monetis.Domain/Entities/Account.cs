using Monetis.Domain.Enums;

namespace Monetis.Domain.Entities;

public class Account : BaseEntity
{
    public string Name { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public AccountType Type { get; private set; }
    public decimal Balance { get; private set; }
    public bool IsNegative => Balance < 0;
    public decimal GetNegativeAmount() => Balance < 0 ? Math.Abs(Balance) : 0;

    protected Account() { }
    
    public Account(string name, Guid userId, AccountType type)
    {
        ValidateCreation(name, userId);
        
        Name = name;
        UserId = userId;
        Type = type;
        Balance = 0;
    }

    public void Deposit(decimal amount)
    {
        ValidateAmountPositive(amount);
        
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        ValidateAmountPositive(amount);
        
        Balance -= amount;
    }

    public void Update(string name)
    {
        ValidateName(name);
        Name = name;
    }

    public void AdjustBalance(decimal newBalance, string reason)
    {
        ValidateAdjustment(reason);
        Balance = newBalance;
    }

    private void ValidateCreation(string name, Guid userId)
    {
        ValidateName(name);
        
        if (userId == Guid.Empty)
            throw new ArgumentException("User is required");
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account name is required");
            
        if (name.Length > 100)
            throw new ArgumentException("Account name must be less than 100 characters");
    }

    private void ValidateAmountPositive(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");
    }

    private void ValidateTransfer(Account destinationAccount, decimal amount)
    {
        if (destinationAccount == null)
            throw new ArgumentException("Destination account is required");
        
        if (destinationAccount.Id == Id)
            throw new ArgumentException("Cannot transfer to the same account");
        
        if (destinationAccount.UserId != UserId)
            throw new ArgumentException("Cannot transfer to an account of a different user");
        
        ValidateAmountPositive(amount);
    }

    private void ValidateAdjustment(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason) || reason.Length < 5)
            throw new ArgumentException("Adjustment reason is required (min 5 characters)");
    }
}