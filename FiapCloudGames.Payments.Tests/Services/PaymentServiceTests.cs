using FiapCloudGames.Payments.Application.InputModels;
using FiapCloudGames.Payments.Application.Interfaces;
using FiapCloudGames.Payments.Application.Services;
using FiapCloudGames.Payments.Application.ViewModels;
using FiapCloudGames.Payments.Domain.Entities;
using FiapCloudGames.Payments.Domain.Exceptions;
using FiapCloudGames.Payments.Infrastructure.Persistence;
using Moq;

namespace FiapCloudGames.Payments.Tests.Services;

public class PaymentServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IPagSeguroService> _pagSeguroService = new();
    private readonly IPaymentService _paymentService;

    public PaymentServiceTests() => _paymentService = new PaymentService(_unitOfWork.Object, _pagSeguroService.Object);

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPayment_WhenPaymentFound()
    {
        // Arrange
        Payment payment = new(1, 10, 100); // supondo construtor: (id, userId, amount)

        _unitOfWork.Setup(u => u.Payments.GetByIdTrackingAsync(It.IsAny<int>()))
                      .ReturnsAsync(payment);

        // Act
        RestResponse<PaymentViewModel> result = await _paymentService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Errors?.Any() ?? false);
        Assert.Equal(payment.PaymentId, result.Data.PaymentId);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowException_WhenPaymentNotFound()
    {
        // Arrange
        _unitOfWork.Setup(u => u.Payments.GetByIdTrackingAsync(It.IsAny<int>()))
                      .ReturnsAsync((Payment?)null);

        // Act & Assert
        await Assert.ThrowsAsync<PaymentNotFoundException>(() => _paymentService.GetByIdAsync(999));
    }

    [Fact]
    public async Task CreateAsync_ShouldCreatePayment_WhenNotFraudulent()
    {
        // Arrange
        CreatePaymentInputModel input = new(1, 100, 50);
        Payment payment = input.ToDomain();

        _pagSeguroService.Setup(p => p.IsFraudulentAsync(It.IsAny<CreatePaymentInputModel>()))
                     .ReturnsAsync(false);
        _pagSeguroService.Setup(p => p.CreateFakePaymentAsync())
                     .ReturnsAsync(Guid.NewGuid());

        _unitOfWork.Setup(u => u.Payments.AddAsync(It.IsAny<Payment>()))
                      .Returns(Task.CompletedTask);

        // Act
        RestResponse<PaymentViewModel> result = await _paymentService.CreateAsync(input);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(input.OrderId, result.Data.OrderId);
        _unitOfWork.Verify(u => u.Payments.AddAsync(It.IsAny<Payment>()), Times.Once);
        _unitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }
}
