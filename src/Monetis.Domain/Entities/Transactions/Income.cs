namespace Monetis.Domain.Entities.Transactions;

public class Income : Transaction
{
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; }
    public DateTime ReceivedAt { get; private set; }

    protected Income() { }

    public Income(Guid userId, Guid accountId, Guid categoryId,
        decimal amount, string description, DateTime receivedAt)
        : base(userId, accountId, amount, description)
    {
        CategoryId = categoryId;
        ReceivedAt = receivedAt;
    }

    public void Update(Guid categoryId, decimal amount, string description, DateTime receivedAt)
    {
        CategoryId = categoryId;
        UpdateBase(amount, description);
        ReceivedAt = receivedAt;
    }
}