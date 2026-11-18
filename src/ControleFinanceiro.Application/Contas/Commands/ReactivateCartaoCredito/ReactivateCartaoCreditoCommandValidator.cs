using FluentValidation;

namespace ControleFinanceiro.Application.Contas.Commands.ReactivateCartaoCredito;

public sealed class ReactivateCartaoCreditoCommandValidator : AbstractValidator<ReactivateCartaoCreditoCommand>
{
    public ReactivateCartaoCreditoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
