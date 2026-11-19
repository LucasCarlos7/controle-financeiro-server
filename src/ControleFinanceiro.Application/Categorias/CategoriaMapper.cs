using ControleFinanceiro.Domain.Categorias;

namespace ControleFinanceiro.Application.Categorias;

internal static class CategoriaMapper
{
    public static CategoriaResponse ToResponse(Categoria categoria) => new()
    {
        Id = categoria.Id,
        Nome = categoria.Nome,
        Tipo = categoria.Tipo,
        Icone = categoria.Icone,
        Cor = categoria.Cor,
        IsPadrao = categoria.IsPadrao,
        Subcategorias = categoria.Subcategorias
            .Select(s => new SubcategoriaResponse { Id = s.Id, Nome = s.Nome })
            .ToList(),
    };
}
