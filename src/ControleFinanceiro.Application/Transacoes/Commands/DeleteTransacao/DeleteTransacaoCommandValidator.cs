using FluentValidation;

namespace ControleFinanceiro.Application.Transacoes.Commands.DeleteTransacao;

public sealed class DeleteTransacaoCommandValidator : AbstractValidator<DeleteTransacaoCommand>
{
    public DeleteTransacaoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
