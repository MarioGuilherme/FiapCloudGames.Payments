using FiapCloudGames.Payments.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FiapCloudGames.Payments.Infrastructure.Persistence;

public class UnitOfWork(FiapCloudGamesPaymentsDbContext dbContext, IPaymentRepository payments) : IUnitOfWork
{
    private readonly FiapCloudGamesPaymentsDbContext _dbContext = dbContext;
    private IDbContextTransaction? _transaction;

    public IPaymentRepository Payments => payments;

    public Task<int> CompleteAsync() => _dbContext.SaveChangesAsync();

    public async Task BeginTransactionAsync() => _transaction = await _dbContext.Database.BeginTransactionAsync();

    public async Task CommitAsync()
    {
        if (_transaction is null)
            return;

        try
        {
            await CompleteAsync();
            await _transaction!.CommitAsync();
        }
        catch
        {
            await _transaction!.RollbackAsync();
            throw;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            _dbContext.Dispose();
    }
}
