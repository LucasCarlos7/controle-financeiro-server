using ControleFinanceiro.Domain.Common;
using FluentValidation;

namespace ControleFinanceiro.Application.Tags.Commands.CreateTag;

public sealed class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Icone).MaximumLength(Icones.TamanhoMaximo);
    }
}
