using ControleFinanceiro.Domain.Categorias;

namespace ControleFinanceiro.Api.Contracts.Categorias;

public sealed record CreateCategoriaRequest
{
    public required string Nome { get; init; }
    public required TipoCategoria Tipo { get; init; }
    public string? Icone { get; init; }
    public string? Cor { get; init; }
}
