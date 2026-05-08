using Monetis.Domain.Entities;

namespace Monetis.Application.Abstractions.Persistence;

public interface ICardRepository : IBaseRepository<Card>
{
    Task<IEnumerable<Card>> GetByUserReadOnlyAsync(Guid userId, CancellationToken cancellationToken = default);
}
