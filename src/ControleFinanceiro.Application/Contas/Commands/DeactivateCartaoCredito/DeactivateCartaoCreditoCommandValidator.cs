using FluentValidation;

namespace ControleFinanceiro.Application.Contas.Commands.DeactivateCartaoCredito;

public sealed class DeactivateCartaoCreditoCommandValidator : AbstractValidator<DeactivateCartaoCreditoCommand>
{
    public DeactivateCartaoCreditoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
