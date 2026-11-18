using FluentValidation;

namespace ControleFinanceiro.Application.Categorias.Commands.DeleteSubcategoria;

public sealed class DeleteSubcategoriaCommandValidator : AbstractValidator<DeleteSubcategoriaCommand>
{
    public DeleteSubcategoriaCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CategoriaId).NotEmpty();
        RuleFor(x => x.SubcategoriaId).NotEmpty();
    }
}
