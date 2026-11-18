using FluentValidation;

namespace ControleFinanceiro.Application.Transacoes.Commands.CreateTransacao;

public sealed class CreateTransacaoCommandValidator : AbstractValidator<CreateTransacaoCommand>
{
    public CreateTransacaoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Valor).GreaterThan(0);
        RuleFor(x => x.Descricao).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Tipo).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
    }
}
