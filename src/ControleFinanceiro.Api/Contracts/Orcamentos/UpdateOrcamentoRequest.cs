namespace ControleFinanceiro.Api.Contracts.Orcamentos;

public sealed record UpdateOrcamentoRequest
{
    public required decimal LimiteValor { get; init; }
}
