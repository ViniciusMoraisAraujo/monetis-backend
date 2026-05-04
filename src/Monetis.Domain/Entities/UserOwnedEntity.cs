namespace Monetis.Domain.Entities;

public class UserOwnedEntity : BaseEntity
{
    public Guid UserId { get; protected set; }
    public User User { get; protected set; }
    
    protected UserOwnedEntity() { }
}