using Monetis.Domain.Enums;

namespace Monetis.Domain.Entities;

public class Account : BaseEntity
{
    public string Name { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public AccountType Type { get; private set; }
    public decimal Balance { get; private set; } = 0;
    public Currency Currency { get; private set; }

    protected Account() { }
    
    public Account(string name, Guid userId, AccountType type, Currency currency)
    {
        Name = name;
        UserId =  userId;
        Type = type;
        Currency = currency;
    }

    public void Update(string name)
    {
        Name = name;
    }
}