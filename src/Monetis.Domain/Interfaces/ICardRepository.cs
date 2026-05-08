using Monetis.Domain.Entities;

namespace Monetis.Domain.Interfaces;

public interface ICardRepository : IBaseRepository<Card>
{
    Task<IEnumerable<Card>> GetByUserReadOnlyAsync(Guid userId, CancellationToken cancellationToken = default);
}
