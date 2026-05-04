namespace Monetis.Domain.Entities;

public class UserOwnedEntity : BaseEntity
{
    public Guid UserId { get; protected set; }
    public User User { get; protected set; }
    
    protected UserOwnedEntity() { }
    
    public void SetUser(Guid userId)
    {
        if (UserId != Guid.Empty)
            throw new InvalidOperationException("UserId já foi definido e não pode ser alterado.");

        UserId = userId;
    }
}