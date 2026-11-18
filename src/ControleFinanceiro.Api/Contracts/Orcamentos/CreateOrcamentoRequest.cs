namespace ControleFinanceiro.Api.Contracts.Orcamentos;

public sealed record CreateOrcamentoRequest
{
    public required Guid CategoriaId { get; init; }
    public required int Ano { get; init; }
    public required int Mes { get; init; }
    public required decimal LimiteValor { get; init; }
}
