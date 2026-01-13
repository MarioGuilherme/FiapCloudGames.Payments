namespace FiapCloudGames.Payments.Domain.Events;

public record class FraudlentOrderDetectedEvent(int OrderId);
