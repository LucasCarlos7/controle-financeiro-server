using FluentValidation;

namespace ControleFinanceiro.Application.Transacoes.Commands.MarkTransacaoAsPendente;

public sealed class MarkTransacaoAsPendenteCommandValidator : AbstractValidator<MarkTransacaoAsPendenteCommand>
{
    public MarkTransacaoAsPendenteCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
