namespace Monetis.Domain.Entities;

public abstract class BaseEntity
{
    //immutable once created
    public Guid Id { get; protected init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected init; } = DateTime.UtcNow;
}
