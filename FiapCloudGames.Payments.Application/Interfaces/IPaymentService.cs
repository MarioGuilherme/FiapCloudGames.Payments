using FiapCloudGames.Payments.Application.InputModels;
using FiapCloudGames.Payments.Application.ViewModels;

namespace FiapCloudGames.Payments.Application.Interfaces;

public interface IPaymentService
{
    Task CancelByIdAsync(int paymentId);
    Task<RestResponse<PaymentViewModel>> CreateAsync(CreatePaymentInputModel inputModel);
    Task<RestResponse<IEnumerable<PaymentViewModel>>> GetAllByUserIdAsync(int userId);
    Task<RestResponse<PaymentViewModel>> GetByIdAsync(int paymentId);
    Task MarkAsPaidAsync(int paymentId);
}
