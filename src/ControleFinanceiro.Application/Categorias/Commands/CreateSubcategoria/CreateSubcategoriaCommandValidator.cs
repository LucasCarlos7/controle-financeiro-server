using FluentValidation;

namespace ControleFinanceiro.Application.Categorias.Commands.CreateSubcategoria;

public sealed class CreateSubcategoriaCommandValidator : AbstractValidator<CreateSubcategoriaCommand>
{
    public CreateSubcategoriaCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CategoriaId).NotEmpty();
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
    }
}
