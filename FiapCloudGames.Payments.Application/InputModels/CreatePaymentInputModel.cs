using FiapCloudGames.Payments.Domain.Entities;

namespace FiapCloudGames.Payments.Application.InputModels;

public record CreatePaymentInputModel(int UserId, int OrderId, decimal Total)
{
    public Payment ToDomain() => new(UserId, OrderId, Total);
};
