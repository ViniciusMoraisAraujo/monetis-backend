using Monetis.Domain.Enums;

namespace Monetis.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; }
    public User? UserId { get; private set; }
    public TransactionType Type { get; private set; }
    public string Icon { get; private set; }
    
}