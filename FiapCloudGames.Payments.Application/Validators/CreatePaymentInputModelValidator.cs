using FiapCloudGames.Payments.Application.InputModels;
using FluentValidation;
using Serilog;

namespace FiapCloudGames.Payments.Application.Validators;

public class CreatePaymentInputModelValidator : AbstractValidator<CreatePaymentInputModel>
{
    public CreatePaymentInputModelValidator()
    {
        Log.Information("Iniciando {ValidatorName}", nameof(CreatePaymentInputModelValidator));

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(inputModel => inputModel.UserId)
            .NotNull().WithMessage("O identificador do usuário da compra precisa ser informado!")
            .GreaterThanOrEqualTo(0).WithMessage("O identificador do usuário é inválido!");

        RuleFor(inputModel => inputModel.OrderId)
            .NotNull().WithMessage("O identificador do usuário da compra precisa ser informado!")
            .GreaterThanOrEqualTo(0).WithMessage("O identificador do usuário é inválido!");

        RuleFor(inputModel => inputModel.Total)
            .NotNull().WithMessage("O valor total da compra precisa ser informado!")
            .GreaterThanOrEqualTo(0).WithMessage("O valor total da compra precisa ser menor que R$ 0,00!");
    }
}
