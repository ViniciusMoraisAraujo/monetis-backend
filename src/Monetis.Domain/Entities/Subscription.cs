using Monetis.Domain.Enums;

namespace Monetis.Domain.Entities;

public class Subscription : BaseEntity
{
    public User UserId { get; private set; }
    public Account AccountId { get; private set; }
    public Category CategoryId { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; }
    public Frequency Frequency { get; private set; }
    public DateTime NextDueDate { get; private set; }
    public bool IsActive { get; private set; }
}