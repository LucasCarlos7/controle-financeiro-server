using ControleFinanceiro.Domain.Common;
using FluentValidation;

namespace ControleFinanceiro.Application.Contas.Commands.UpdateCartaoCredito;

public sealed class UpdateCartaoCreditoCommandValidator : AbstractValidator<UpdateCartaoCreditoCommand>
{
    public UpdateCartaoCreditoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Limite).GreaterThan(0);
        RuleFor(x => x.DiaFechamento).InclusiveBetween(1, 31);
        RuleFor(x => x.DiaVencimento).InclusiveBetween(1, 31);
        RuleFor(x => x.Icone).MaximumLength(Icones.TamanhoMaximo);
    }
}
