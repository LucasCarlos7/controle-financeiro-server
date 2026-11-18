using FluentValidation;

namespace ControleFinanceiro.Application.Categorias.Commands.DeleteCategoria;

public sealed class DeleteCategoriaCommandValidator : AbstractValidator<DeleteCategoriaCommand>
{
    public DeleteCategoriaCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
