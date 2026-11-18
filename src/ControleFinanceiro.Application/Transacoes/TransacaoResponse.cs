using ControleFinanceiro.Domain.Transacoes;

namespace ControleFinanceiro.Application.Transacoes;

public sealed record TransacaoResponse
{
    public required Guid Id { get; init; }
    public required TipoTransacao Tipo { get; init; }
    public required decimal Valor { get; init; }
    public required DateOnly Data { get; init; }
    public required string Descricao { get; init; }
    public Guid? ContaId { get; init; }
    public Guid? CartaoCreditoId { get; init; }
    public Guid? ContaDestinoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public required StatusTransacao Status { get; init; }
    public Guid? RecorrenciaId { get; init; }
    public required IReadOnlyList<Guid> TagIds { get; init; }
}
