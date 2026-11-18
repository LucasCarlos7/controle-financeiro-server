namespace ControleFinanceiro.Application.Objetivos;

public sealed record ObjetivoResponse
{
    public required Guid Id { get; init; }
    public required string Nome { get; init; }
    public required decimal ValorAlvo { get; init; }
    public required decimal ValorAtual { get; init; }
    public required decimal PercentualConcluido { get; init; }
    public required DateOnly DataAlvo { get; init; }
    public required IReadOnlyList<AporteResponse> Aportes { get; init; }
}

public sealed record AporteResponse
{
    public required Guid Id { get; init; }
    public required decimal Valor { get; init; }
    public required DateOnly Data { get; init; }
}
