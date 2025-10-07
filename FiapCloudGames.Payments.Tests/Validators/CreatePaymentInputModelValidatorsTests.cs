using FiapCloudGames.Payments.Application.InputModels;
using FiapCloudGames.Payments.Application.Validators;
using FluentValidation.Results;

namespace FiapCloudGames.Payments.Tests.Validators;

public class CreatePaymentInputModelValidatorsTests
{
    [Fact]
    public void Should_PassValidation_When_InputModelIsValid()
    {
        // Arrange
        CreatePaymentInputModel input = new(1, 10, 100);

        // Act
        ValidationResult result = new CreatePaymentInputModelValidator().Validate(input);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Should_FailValidation_When_UserIdIsNegative()
    {
        CreatePaymentInputModel input = new(-1, 10, 100);

        ValidationResult result = new CreatePaymentInputModelValidator().Validate(input);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "UserId");
    }
}
