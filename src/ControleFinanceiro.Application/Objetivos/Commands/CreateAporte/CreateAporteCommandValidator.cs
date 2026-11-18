using FluentValidation;

namespace ControleFinanceiro.Application.Objetivos.Commands.CreateAporte;

public sealed class CreateAporteCommandValidator : AbstractValidator<CreateAporteCommand>
{
    public CreateAporteCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ObjetivoId).NotEmpty();
        RuleFor(x => x.Valor).GreaterThan(0);
    }
}
