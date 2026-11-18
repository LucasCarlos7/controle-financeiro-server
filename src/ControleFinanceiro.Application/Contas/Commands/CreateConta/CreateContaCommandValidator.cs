using ControleFinanceiro.Domain.Common;
using FluentValidation;

namespace ControleFinanceiro.Application.Contas.Commands.CreateConta;

public sealed class CreateContaCommandValidator : AbstractValidator<CreateContaCommand>
{
    public CreateContaCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Tipo).IsInEnum();
        RuleFor(x => x.Icone).MaximumLength(Icones.TamanhoMaximo);
    }
}
