using FiapCloudGames.Payments.Application.InputModels;
using FiapCloudGames.Payments.Application.Interfaces;
using FiapCloudGames.Payments.Application.ViewModels;
using FiapCloudGames.Payments.Domain.Entities;
using FiapCloudGames.Payments.Domain.Events;
using FiapCloudGames.Payments.Domain.Exceptions;
using FiapCloudGames.Payments.Domain.Messaging;
using FiapCloudGames.Payments.Infrastructure.Persistence;
using Serilog;

namespace FiapCloudGames.Payments.Application.Services;

public class PaymentService(IUnitOfWork unitOfWork, IEventPublisher eventPublisher, IPagSeguroService pagSeguroService) : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEventPublisher _eventPublisher = eventPublisher;
    private readonly IPagSeguroService _pagSeguroService = pagSeguroService;

    public async Task CancelByIdAsync(int paymentId)
    {
        Log.Information("Iniciando cancelamento do pagamento {paymentId}", paymentId);
        Payment? payment = await _unitOfWork.Payments.GetByIdTrackingAsync(paymentId);
        if (payment is null)
        {
            Log.Warning("Pagamento {paymentId} não encontrado", paymentId);
            throw new PaymentNotFoundException();
        }

        payment.Cancel();
        Log.Information("Pagamento {paymentId} cancelado com sucesso", paymentId);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<PaymentViewModel> CreateAsync(CreatePaymentInputModel inputModel)
    {
        Log.Information("Iniciando criação do pagamento com id de pedido {orderId}", inputModel.OrderId);

        Payment payment = inputModel.ToDomain();

        Log.Information("Verificando se a compra é fraudulenta para o pedido de id {orderId}", inputModel.OrderId);
        bool isFraudulent = await _pagSeguroService.IsFraudulentAsync(inputModel);
        if (isFraudulent)
        {
            Log.Warning("Compra fraudulenta detectada para o id de pedido {orderId}", inputModel.OrderId);
            throw new PaymentFraudDetectedException();
        }

        Log.Information("Compra verificada com sucesso. Criando pagamento para geração de ExternalId na API do PagSeguro");
        Guid externalId = await _pagSeguroService.CreateFakePaymentAsync();

        Log.Information("ExternalId {externalId} criado com sucesso para o id de pedido {orderId}", externalId.ToString(), inputModel.OrderId);

        payment.UpdateExternalId(externalId);
        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.CompleteAsync();
        Log.Information("Pagamento criado com sucesso para o id de pedido {orderId}", inputModel.OrderId);

        return PaymentViewModel.FromDomain(payment);
    }

    public async Task<RestResponse<IEnumerable<PaymentViewModel>>> GetAllByUserIdAsync(int userId)
    {
        Log.Information("Buscando todos os pagamentos do usuário {userId}", userId);
        List<Payment> payments = await _unitOfWork.Payments.GetAllByUserIdAsync(userId);
        Log.Information("Total de {Total} Pagamentos do usuário {userId} buscados com sucesso", payments.Count, userId);
        return RestResponse<IEnumerable<PaymentViewModel>>.Success(payments.Select(PaymentViewModel.FromDomain));
    }

    public async Task<RestResponse<PaymentViewModel>> GetByIdAsync(int paymentId)
    {
        Log.Information("Buscando pagamento {paymentId}", paymentId);
        Payment? payment = await _unitOfWork.Payments.GetByIdTrackingAsync(paymentId);
        if (payment is null)
        {
            Log.Warning("Pagamento {paymentId} não encontrado", paymentId);
            throw new PaymentNotFoundException();
        }

        Log.Information("Pagamento {paymentId} buscado com sucesso", paymentId);
        return RestResponse<PaymentViewModel>.Success(PaymentViewModel.FromDomain(payment));
    }

    public async Task MarkAsPaidAsync(int paymentId)
    {
        Log.Information("Marcando pagamento {paymentId} como pago", paymentId);
        Payment? payment = await _unitOfWork.Payments.GetByIdTrackingAsync(paymentId);
        if (payment is null)
        {
            Log.Warning("Pagamento {paymentId} não encontrado", paymentId);
            throw new PaymentNotFoundException();
        }

        payment.MarkAsPaid();
        Log.Information("Pagamento {paymentId} marcado como pago com sucesso", paymentId);
        await _unitOfWork.CompleteAsync();
    }

    public async Task ProcessWebHookAsync(ReceivedPaymentEvent receivedPaymentEvent)
    {
        Payment? payment = await _unitOfWork.Payments.GetByExternalIdTrackingAsync(receivedPaymentEvent.ExternalId);

        if (payment is null)
        {
            Log.Warning("Pagamento {paymentId} não encontrado", payment?.PaymentId);
            throw new PaymentNotFoundException();
        }

        Log.Information("Marcando pagamento {paymentId} como pago", payment?.PaymentId);
        payment!.MarkAsPaid();

        await _unitOfWork.CompleteAsync();
        Log.Information("Pagamento {paymentId} marcado como pago com sucesso", payment.PaymentId);

        await _eventPublisher.PublishAsync(new PaidOrderEvent(payment.OrderId), "paid.order");
    }
}
