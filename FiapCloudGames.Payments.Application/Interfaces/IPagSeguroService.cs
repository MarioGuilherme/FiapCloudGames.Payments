using FiapCloudGames.Payments.Application.InputModels;

namespace FiapCloudGames.Payments.Application.Interfaces;

public interface IPagSeguroService
{
    Task<Guid> CreateFakePaymentAsync();
    Task<bool> IsFraudulentAsync(CreatePaymentInputModel inputModel);
}
