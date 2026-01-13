namespace FiapCloudGames.Payments.Domain.Events;

public record OrderPendingPaymentCreatedEvent(int OrderId, int UserId, int PaymentId);
