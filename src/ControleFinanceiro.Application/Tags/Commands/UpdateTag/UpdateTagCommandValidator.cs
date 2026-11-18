using ControleFinanceiro.Domain.Common;
using FluentValidation;

namespace ControleFinanceiro.Application.Tags.Commands.UpdateTag;

public sealed class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Icone).MaximumLength(Icones.TamanhoMaximo);
    }
}
