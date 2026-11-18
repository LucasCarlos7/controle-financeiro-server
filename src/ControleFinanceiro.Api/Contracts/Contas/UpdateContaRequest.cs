namespace ControleFinanceiro.Api.Contracts.Contas;

public sealed record UpdateContaRequest
{
    public required string Nome { get; init; }
    public string? Icone { get; init; }
}
