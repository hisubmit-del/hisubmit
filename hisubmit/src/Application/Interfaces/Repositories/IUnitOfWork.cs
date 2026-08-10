using HiSubmit.Domain.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Interfaces.Repositories;

public interface IUnitOfWork<TId> : IDisposable
{
    IRepositoryAsync<T, TId> Repository<T>() where T : AuditableEntity<TId>;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

    Task RollbackTransactions();
    Task BeginTransaction();
    Task CommitTransaction();
}
