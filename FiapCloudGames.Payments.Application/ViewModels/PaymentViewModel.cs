using FiapCloudGames.Payments.Domain.Entities;

namespace FiapCloudGames.Payments.Application.ViewModels;

public record PaymentViewModel(int PaymentId, int UserId, int OrderId, decimal Total, DateTime? PaidAt = default, DateTime? CanceledAt = default, Guid? ExternalId = default)
{
    public static PaymentViewModel FromDomain(Payment payment) => new(
        payment.PaymentId,
        payment.UserId,
        payment.OrderId,
        payment.Total,
        payment.PaidAt,
        payment.CanceledAt,
        payment.ExternalId
    );
};
