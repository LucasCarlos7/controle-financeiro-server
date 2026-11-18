using FluentValidation;

namespace ControleFinanceiro.Application.Orcamentos.Commands.UpdateOrcamento;

public sealed class UpdateOrcamentoCommandValidator : AbstractValidator<UpdateOrcamentoCommand>
{
    public UpdateOrcamentoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.LimiteValor).GreaterThan(0);
    }
}
