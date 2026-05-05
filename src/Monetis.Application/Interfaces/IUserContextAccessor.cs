namespace Monetis.Application.Interfaces;

public interface IUserContextAccessor
{
    Guid UserId { get; }
    bool IsResolved { get; }
}
