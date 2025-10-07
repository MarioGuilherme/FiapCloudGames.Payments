namespace FiapCloudGames.Payments.Domain.Entities;

public class PaymentEvent
{
    public int PaymentEventId { get; private set; }
    public int PaymentId { get; private set; }
    public Payment Payment { get; private set; } = null!;
    public int UserId { get; private set; }
    public int OrderId { get; private set; }
    public Guid? ExternalId { get; private set; }
    public decimal Total { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? CanceledAt { get; private set; }
    public DateTime EventAt { get; } = DateTime.Now;

    private PaymentEvent() { }

    public static PaymentEvent FromPayment(Payment payment) => new PaymentEvent
    {
        PaymentId = payment.PaymentId,
        UserId = payment.UserId,
        OrderId = payment.OrderId,
        ExternalId = payment.ExternalId,
        Total = payment.Total,
        PaidAt = payment.PaidAt,
        CanceledAt = payment.CanceledAt
    };
}
