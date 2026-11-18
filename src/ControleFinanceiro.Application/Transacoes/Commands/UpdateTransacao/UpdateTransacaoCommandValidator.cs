using FluentValidation;

namespace ControleFinanceiro.Application.Transacoes.Commands.UpdateTransacao;

public sealed class UpdateTransacaoCommandValidator : AbstractValidator<UpdateTransacaoCommand>
{
    public UpdateTransacaoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Valor).GreaterThan(0);
        RuleFor(x => x.Descricao).NotEmpty().MaximumLength(200);
    }
}
