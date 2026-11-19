using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Categorias;

public static class CategoriaErrors
{
    public static readonly Error NomeObrigatorio =
        new("Categoria.NomeObrigatorio", "O nome da categoria é obrigatório.");

    public static readonly Error NomeSubcategoriaObrigatorio =
        new("Categoria.NomeSubcategoriaObrigatorio", "O nome da subcategoria é obrigatório.");

    public static readonly Error NaoEncontrada =
        new("Categoria.NaoEncontrada", "Categoria não encontrada.");

    public static readonly Error SubcategoriaNaoEncontrada =
        new("Categoria.SubcategoriaNaoEncontrada", "Subcategoria não encontrada.");

    public static readonly Error SubcategoriaNaoPertenceACategoria =
        new("Categoria.SubcategoriaNaoPertenceACategoria", "A subcategoria informada não pertence à categoria informada.");

    public static readonly Error ExclusaoDeCategoriaPadrao =
        new("Categoria.ExclusaoDeCategoriaPadrao", "Categorias padrão do sistema não podem ser excluídas.");
}
