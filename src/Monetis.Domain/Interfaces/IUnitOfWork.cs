namespace Monetis.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<bool> CommitAsync(CancellationToken cancellationToken = default);
}