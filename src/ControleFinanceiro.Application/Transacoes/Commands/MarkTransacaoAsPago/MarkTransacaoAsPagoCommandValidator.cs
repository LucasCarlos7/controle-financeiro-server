using FluentValidation;

namespace ControleFinanceiro.Application.Transacoes.Commands.MarkTransacaoAsPago;

public sealed class MarkTransacaoAsPagoCommandValidator : AbstractValidator<MarkTransacaoAsPagoCommand>
{
    public MarkTransacaoAsPagoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
