using FiapCloudGames.Payments.Domain.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualBasic;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FiapCloudGames.Payments.Infrastructure.Messaging.RabbitMq;

public class RabbitMqPublisher(IOptions<RabbitMqOptions> options, IRabbitMqConnection connection, IHttpContextAccessor httpContextAccessor) : IEventPublisher
{
    private readonly RabbitMqOptions _options = options.Value;
    private readonly IRabbitMqConnection _connection = connection;

    public async Task PublishAsync<TEvent>(TEvent @event, string routingKey, string? correlationId = default)
    {
        using IChannel channel = await (await _connection.GetConnectionAsync()).CreateChannelAsync();

        await channel.ExchangeDeclareAsync(_options.Exchange, ExchangeType.Topic, durable: true);

        byte[] body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
        if (httpContextAccessor.HttpContext is not null)
        {
            httpContextAccessor.HttpContext!.Response.Headers.TryGetValue("X-Correlation-ID", out StringValues stringValues);
            correlationId = stringValues.ToString();
        }
        BasicProperties basicProperties = new()
        {
            CorrelationId = correlationId
        };

        await channel.BasicPublishAsync(_options.Exchange, routingKey, false, basicProperties, body);
    }
}
