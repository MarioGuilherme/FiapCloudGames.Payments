using RabbitMQ.Client;

namespace FiapCloudGames.Payments.Infrastructure.Messaging.RabbitMq;

public interface IRabbitMqConnection
{
    Task<IConnection> GetConnectionAsync();
}