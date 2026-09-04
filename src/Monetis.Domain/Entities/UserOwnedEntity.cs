using Monetis.Domain.Exceptions;

namespace Monetis.Domain.Entities;

public class UserOwnedEntity : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; init; } = null!;

    protected UserOwnedEntity()
    {
        //required for ef
    }

    public void SetUser(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId não pode ser vazio.", nameof(userId));

        if (userId == UserId)
            return;

        if (UserId != Guid.Empty)
            throw new UserOwnedEntityUserAlreadySetException();

        UserId = userId;
    }
}
