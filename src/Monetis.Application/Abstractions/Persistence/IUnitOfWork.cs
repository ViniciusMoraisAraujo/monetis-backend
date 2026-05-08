namespace Monetis.Application.Abstractions.Persistence;

public interface IUnitOfWork : IDisposable
{
    Task<bool> CommitAsync(CancellationToken cancellationToken = default);
}