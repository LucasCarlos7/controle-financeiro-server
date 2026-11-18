using FluentValidation;

namespace ControleFinanceiro.Application.Categorias.Commands.UpdateSubcategoria;

public sealed class UpdateSubcategoriaCommandValidator : AbstractValidator<UpdateSubcategoriaCommand>
{
    public UpdateSubcategoriaCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CategoriaId).NotEmpty();
        RuleFor(x => x.SubcategoriaId).NotEmpty();
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
    }
}
