using Monetis.Application.Abstractions.Security;

namespace Monetis.Infrastructure.Security;

public class UserContextAccessor(UserContext userContext) : IUserContextAccessor
{
    public Guid UserId => userContext.UserId;
    public bool IsResolved => userContext.IsResolved;
}
