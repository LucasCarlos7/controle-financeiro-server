using ControleFinanceiro.Domain.Common;
using FluentValidation;

namespace ControleFinanceiro.Application.Contas.Commands.UpdateConta;

public sealed class UpdateContaCommandValidator : AbstractValidator<UpdateContaCommand>
{
    public UpdateContaCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Icone).MaximumLength(Icones.TamanhoMaximo);
    }
}
