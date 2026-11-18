using ControleFinanceiro.Domain.Categorias;

namespace ControleFinanceiro.Application.Categorias;

public sealed record CategoriaResponse
{
    public required Guid Id { get; init; }
    public required string Nome { get; init; }
    public required TipoCategoria Tipo { get; init; }
    public string? Icone { get; init; }
    public string? Cor { get; init; }
    public required IReadOnlyList<SubcategoriaResponse> Subcategorias { get; init; }
}

public sealed record SubcategoriaResponse
{
    public required Guid Id { get; init; }
    public required string Nome { get; init; }
}
