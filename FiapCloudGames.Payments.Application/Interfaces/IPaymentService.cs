using FiapCloudGames.Payments.Application.InputModels;
using FiapCloudGames.Payments.Application.ViewModels;
using FiapCloudGames.Payments.Domain.Events;

namespace FiapCloudGames.Payments.Application.Interfaces;

public interface IPaymentService
{
    Task CancelByIdAsync(int paymentId);
    Task<PaymentViewModel> CreateAsync(CreatePaymentInputModel inputModel);
    Task<RestResponse<IEnumerable<PaymentViewModel>>> GetAllByUserIdAsync(int userId);
    Task<RestResponse<PaymentViewModel>> GetByIdAsync(int paymentId);
    Task MarkAsPaidAsync(int paymentId);
    Task ProcessWebHookAsync(ReceivedPaymentEvent receivedPaymentEvent);
}
