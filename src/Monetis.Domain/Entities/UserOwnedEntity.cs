using Monetis.Domain.Exceptions;

namespace Monetis.Domain.Entities;

public class UserOwnedEntity : BaseEntity
{
    public Guid UserId { get; protected set; }
    public User User { get; init; } = null!;

    protected UserOwnedEntity()
    {
        //required for ef
    }

    public void SetUser(Guid userId)
    {
        if (UserId != Guid.Empty)
            throw new UserOwnedEntityUserAlreadySetException();

        UserId = userId;
    }
}
