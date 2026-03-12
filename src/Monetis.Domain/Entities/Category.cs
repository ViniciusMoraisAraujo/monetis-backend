using Monetis.Domain.Enums;

namespace Monetis.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; }
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public TransactionType Type { get; private set; }
    public string Icon { get; private set; }
    
    protected Category() { }
    
    public Category(string name, Guid userId, TransactionType type, string icon)
    {
        Name = name;
        UserId = userId;
        Type = type;
        Icon = icon;
    }
}