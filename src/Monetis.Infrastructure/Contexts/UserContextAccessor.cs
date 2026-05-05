using Monetis.Application.Interfaces;

namespace Monetis.Infrastructure.Contexts;

public class UserContextAccessor(UserContext userContext) : IUserContextAccessor
{
    public Guid UserId => userContext.UserId;
    public bool IsResolved => userContext.IsResolved;
}
