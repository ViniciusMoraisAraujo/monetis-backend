namespace Monetis.Application.Abstractions.Security;

public interface IUserContextAccessor
{
    Guid UserId { get; }
    bool IsResolved { get; }
}
