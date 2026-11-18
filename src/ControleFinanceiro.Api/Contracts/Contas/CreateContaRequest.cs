using ControleFinanceiro.Domain.Contas;

namespace ControleFinanceiro.Api.Contracts.Contas;

public sealed record CreateContaRequest
{
    public required string Nome { get; init; }
    public required TipoConta Tipo { get; init; }
    public decimal SaldoInicial { get; init; }
    public string? Icone { get; init; }
}
