using FluentValidation;

namespace ControleFinanceiro.Application.Objetivos.Commands.UpdateObjetivo;

public sealed class UpdateObjetivoCommandValidator : AbstractValidator<UpdateObjetivoCommand>
{
    public UpdateObjetivoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ValorAlvo).GreaterThan(0);
    }
}
