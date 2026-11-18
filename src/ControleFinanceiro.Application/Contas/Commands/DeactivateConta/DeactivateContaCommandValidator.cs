using FluentValidation;

namespace ControleFinanceiro.Application.Contas.Commands.DeactivateConta;

public sealed class DeactivateContaCommandValidator : AbstractValidator<DeactivateContaCommand>
{
    public DeactivateContaCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
