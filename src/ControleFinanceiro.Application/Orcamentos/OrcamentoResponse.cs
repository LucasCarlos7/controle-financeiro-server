namespace ControleFinanceiro.Application.Orcamentos;

public sealed record OrcamentoResponse
{
    public required Guid Id { get; init; }
    public required Guid CategoriaId { get; init; }
    public required int Ano { get; init; }
    public required int Mes { get; init; }
    public required decimal LimiteValor { get; init; }
    public required decimal ValorConsumido { get; init; }

    /// <summary>Parte de <see cref="ValorConsumido"/> que ainda está Pendente.</summary>
    public required decimal ValorPendente { get; init; }
    public required decimal PercentualConsumido { get; init; }
    public required bool Estourado { get; init; }
}
