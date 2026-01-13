namespace FiapCloudGames.Payments.Domain.Events;

public record OrderCreatedEvent(int OrderId, int UserId, decimal Total);