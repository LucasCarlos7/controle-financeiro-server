namespace ControleFinanceiro.Api.Contracts.Categorias;

public sealed record SubcategoriaRequest
{
    public required string Nome { get; init; }
}
