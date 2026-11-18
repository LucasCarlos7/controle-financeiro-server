using FluentValidation;

namespace ControleFinanceiro.Application.Objetivos.Commands.DeleteObjetivo;

public sealed class DeleteObjetivoCommandValidator : AbstractValidator<DeleteObjetivoCommand>
{
    public DeleteObjetivoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
