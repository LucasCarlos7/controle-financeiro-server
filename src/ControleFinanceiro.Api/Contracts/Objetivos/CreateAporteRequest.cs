namespace ControleFinanceiro.Api.Contracts.Objetivos;

public sealed record CreateAporteRequest
{
    public required decimal Valor { get; init; }
    public required DateOnly Data { get; init; }
}
