namespace Monetis.Domain.Entities;

public class CreditCard : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; }

    protected CreditCard() { }

    public CreditCard(Guid userId, string name)
    {
        UserId = userId;
        Name = name;
    }
}