using FiapCloudGames.Payments.Application.InputModels;
using FiapCloudGames.Payments.Application.Interfaces;
using FiapCloudGames.Payments.Application.ViewModels;
using FiapCloudGames.Payments.Domain.Events;
using FiapCloudGames.Payments.Domain.Exceptions;
using FiapCloudGames.Payments.Domain.Messaging;
using FiapCloudGames.Payments.Infrastructure.Messaging.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Serilog.Context;
using System.Text;
using System.Text.Json;

namespace FiapCloudGames.Payments.Application.Subscribers;

public class OrderCreatedSubscriber(IServiceProvider serviceProvider,
    IOptions<RabbitMqOptions> options,
    IRabbitMqConnection connection,
    IEventPublisher eventPublisher) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly RabbitMqOptions _options = options.Value;
    private readonly IRabbitMqConnection _connection = connection;
    private readonly IEventPublisher _eventPublisher = eventPublisher;
    private const string QUEUE = "order-created";
    private const string ROUTING_KEY = "order.created";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IChannel channel = await (await _connection.GetConnectionAsync()).CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(_options.Exchange, ExchangeType.Topic, true, cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync(QUEUE, true, false, false, cancellationToken: stoppingToken);
        await channel.QueueBindAsync(QUEUE, _options.Exchange, ROUTING_KEY, cancellationToken: stoppingToken);
        AsyncEventingBasicConsumer consumer = new(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            string message = Encoding.UTF8.GetString(ea.Body.ToArray());

            OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message)!;

            using (LogContext.PushProperty("CorrelationId", ea.BasicProperties.CorrelationId))
            {
                await ProcessOrderCreatedAsync(orderCreatedEvent, ea.BasicProperties.CorrelationId!);
            }

            await channel.BasicAckAsync(ea.DeliveryTag, false);
        };

        await channel.BasicConsumeAsync(QUEUE, false, consumer, cancellationToken: stoppingToken);
    }

    private async Task ProcessOrderCreatedAsync(OrderCreatedEvent orderCreatedEvent, string correlationId)
    {
        Log.Information("Subscriber {SubscriberName} iniciado às {DateTime}", nameof(OrderCreatedSubscriber), DateTime.Now);

        using IServiceScope scope = _serviceProvider.CreateScope();
        IPaymentService paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        CreatePaymentInputModel createPaymentInputModel = new(orderCreatedEvent.UserId, orderCreatedEvent.OrderId, orderCreatedEvent.Total);

        try
        {
            PaymentViewModel paymentViewModel = await paymentService.CreateAsync(createPaymentInputModel);

            Log.Information("ExternalId {externalId} criado com sucesso para o id de pedido {orderId}", paymentViewModel.ExternalId, paymentViewModel.OrderId);

            await _eventPublisher.PublishAsync(new OrderPendingPaymentCreatedEvent(orderCreatedEvent.OrderId, orderCreatedEvent.UserId, paymentViewModel.PaymentId), "order.pending.payment.created", correlationId);
        }
        catch (PaymentFraudDetectedException)
        {
            await paymentService.CancelByIdAsync(orderCreatedEvent.OrderId);
            await _eventPublisher.PublishAsync(new SendPendingEmailEvent(orderCreatedEvent.UserId, "Compra fraudulenta", "Sua compra foi identificada como fraudulenta"), "send.pending.email", correlationId);
            await _eventPublisher.PublishAsync(new FraudlentOrderDetectedEvent(orderCreatedEvent.OrderId), "fraudlent.order.detected", correlationId);
            Log.Warning("O Pedido de Id {OrderId} foi identificado como fraudulento. Iniciando pedido de cancelamento", orderCreatedEvent.OrderId);
        }

        Log.Information("Subscriber {SubscriberName} finalizado às {DateTime}", nameof(OrderCreatedSubscriber), DateTime.Now);
    }
}
