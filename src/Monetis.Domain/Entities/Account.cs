using Monetis.Domain.Enums;

namespace Monetis.Domain.Entities;

public class Account : UserOwnedEntity
{
    public string Name { get; private set; }
    public AccountType Type { get; private set; }
    public decimal Balance { get; private set; }
    public bool IsNegative => Balance < 0;
    public decimal GetNegativeAmount() => Balance < 0 ? Math.Abs(Balance) : 0;

    protected Account() { }
    
    public Account(string name,  AccountType type)
    {
        ValidateCreation(name);
        
        Name = name;
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

    private void ValidateCreation(string name)
    {
        ValidateName(name);
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

    private void ValidateAdjustment(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason) || reason.Length < 5)
            throw new ArgumentException("Adjustment reason is required (min 5 characters)");
    }
}