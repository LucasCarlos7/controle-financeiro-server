using FluentValidation;

namespace ControleFinanceiro.Application.Transacoes.Commands.CreateTransacaoRecorrente;

public sealed class CreateTransacaoRecorrenteCommandValidator : AbstractValidator<CreateTransacaoRecorrenteCommand>
{
    public CreateTransacaoRecorrenteCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Valor).GreaterThan(0);
        RuleFor(x => x.Descricao).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Tipo).IsInEnum();
        RuleFor(x => x.TipoRecorrencia).IsInEnum();

        RuleFor(x => x.IntervaloMeses)
            .InclusiveBetween(1, 12)
            .When(x => x.IntervaloMeses is not null);

        // Opcional: sem valor, o handler gera TotalParcelasPadrao lançamentos.
        RuleFor(x => x.TotalParcelas)
            .InclusiveBetween(1, 120)
            .When(x => x.TotalParcelas is not null);
    }
}
