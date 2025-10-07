using FiapCloudGames.Payments.Domain.Entities;

namespace FiapCloudGames.Payments.Domain.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment);
    Task<List<Payment>> GetAllByUserIdAsync(int userId);
    Task<Payment?> GetByIdTrackingAsync(int paymentId);
}
