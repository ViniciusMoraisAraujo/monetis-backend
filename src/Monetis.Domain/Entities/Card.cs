namespace Monetis.Domain.Entities;

public class Card : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; }

    protected Card() { }

    public Card(Guid userId, string name)
    {
        UserId = userId;
        Name = name;
    }
}