namespace ControleFinanceiro.Api.Contracts.Objetivos;

public sealed record CreateObjetivoRequest
{
    public required string Nome { get; init; }
    public required decimal ValorAlvo { get; init; }
    public required DateOnly DataAlvo { get; init; }
}
