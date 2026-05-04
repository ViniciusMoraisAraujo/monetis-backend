namespace Monetis.Infrastructure.Contexts;

public class UserContext
{
    public Guid UserId { get; private set; }
    public bool IsResolved { get; private set; }

    public void SetUser(Guid userId)
    {
        UserId = userId;
        IsResolved = true;
    }
}