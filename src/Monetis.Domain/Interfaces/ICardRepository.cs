using Monetis.Domain.Entities;

namespace Monetis.Domain.Interfaces;

public interface ICardRepository : IRepository<Card>
{
    Task<IEnumerable<Card>> GetByUserReadOnlyAsync(Guid userId);
}
