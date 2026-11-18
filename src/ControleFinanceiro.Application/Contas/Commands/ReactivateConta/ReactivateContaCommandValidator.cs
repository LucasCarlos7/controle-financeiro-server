using FluentValidation;

namespace ControleFinanceiro.Application.Contas.Commands.ReactivateConta;

public sealed class ReactivateContaCommandValidator : AbstractValidator<ReactivateContaCommand>
{
    public ReactivateContaCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
