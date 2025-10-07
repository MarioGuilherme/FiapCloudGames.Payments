using FiapCloudGames.Payments.Domain.Entities;
using FiapCloudGames.Payments.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Payments.Infrastructure.Persistence.Repositories;

public class PaymentRepository(FiapCloudGamesPaymentsDbContext dbContext) : IPaymentRepository
{
    private readonly FiapCloudGamesPaymentsDbContext _dbContext = dbContext;

    public async Task AddAsync(Payment payment)
    {
        await _dbContext.Payments.AddAsync(payment);
    }

    public Task<List<Payment>> GetAllByUserIdAsync(int userId) => _dbContext.Payments
        .AsNoTracking()
        .Where(p => p.UserId == userId)
        .ToListAsync();

    public Task<Payment?> GetByIdTrackingAsync(int paymentId) => _dbContext.Payments
        .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
}
