using Monetis.Domain.Enums;

namespace Monetis.Domain.Entities;

public class Account : BaseEntity
{
    public string Name { get; private set; }
    public User UserId { get; private set; }
    public AccountType Type { get; private set; }
    public decimal Balance { get; private set; }
    public Currency Currency { get; private set; }


    public Account(string name, User userId, AccountType type, decimal balance, Currency currency)
    {
        Name = name;
        UserId = userId;
        Type = type;
        Balance = balance;
        Currency = currency;
    }
}