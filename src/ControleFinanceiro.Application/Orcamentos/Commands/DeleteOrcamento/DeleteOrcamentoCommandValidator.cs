using FluentValidation;

namespace ControleFinanceiro.Application.Orcamentos.Commands.DeleteOrcamento;

public sealed class DeleteOrcamentoCommandValidator : AbstractValidator<DeleteOrcamentoCommand>
{
    public DeleteOrcamentoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
