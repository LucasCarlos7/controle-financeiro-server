using FluentValidation;

namespace ControleFinanceiro.Application.Objetivos.Commands.CreateObjetivo;

public sealed class CreateObjetivoCommandValidator : AbstractValidator<CreateObjetivoCommand>
{
    public CreateObjetivoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ValorAlvo).GreaterThan(0);
    }
}
