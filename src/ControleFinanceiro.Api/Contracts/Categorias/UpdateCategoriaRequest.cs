namespace ControleFinanceiro.Api.Contracts.Categorias;

public sealed record UpdateCategoriaRequest
{
    public required string Nome { get; init; }
    public string? Icone { get; init; }
    public string? Cor { get; init; }
}
