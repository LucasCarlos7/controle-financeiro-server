using FluentValidation;

namespace ControleFinanceiro.Application.Objetivos.Commands.DeleteAporte;

public sealed class DeleteAporteCommandValidator : AbstractValidator<DeleteAporteCommand>
{
    public DeleteAporteCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ObjetivoId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
