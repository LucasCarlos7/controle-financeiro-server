using FluentValidation;

namespace ControleFinanceiro.Application.Transacoes.Commands.DeleteRecorrencia;

public sealed class DeleteRecorrenciaCommandValidator : AbstractValidator<DeleteRecorrenciaCommand>
{
    public DeleteRecorrenciaCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
