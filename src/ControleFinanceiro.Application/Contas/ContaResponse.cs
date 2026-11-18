using ControleFinanceiro.Domain.Contas;

namespace ControleFinanceiro.Application.Contas;

public sealed record ContaResponse
{
    public required Guid Id { get; init; }
    public required string Nome { get; init; }
    public required TipoConta Tipo { get; init; }
    public required decimal SaldoInicial { get; init; }
    public required decimal SaldoAtual { get; init; }
    public string? Icone { get; init; }
    public required bool Ativo { get; init; }
}
