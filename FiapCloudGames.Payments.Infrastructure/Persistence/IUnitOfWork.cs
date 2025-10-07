using FiapCloudGames.Payments.Domain.Repositories;

namespace FiapCloudGames.Payments.Infrastructure.Persistence;

public interface IUnitOfWork : IDisposable
{
    IPaymentRepository Payments { get; }
    Task<int> CompleteAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
}
