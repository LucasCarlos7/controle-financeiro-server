using FluentValidation;

namespace ControleFinanceiro.Application.Orcamentos.Commands.CreateOrcamento;

public sealed class CreateOrcamentoCommandValidator : AbstractValidator<CreateOrcamentoCommand>
{
    public CreateOrcamentoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CategoriaId).NotEmpty();
        RuleFor(x => x.Ano).GreaterThanOrEqualTo(2000);
        RuleFor(x => x.Mes).InclusiveBetween(1, 12);
        RuleFor(x => x.LimiteValor).GreaterThan(0);
    }
}
