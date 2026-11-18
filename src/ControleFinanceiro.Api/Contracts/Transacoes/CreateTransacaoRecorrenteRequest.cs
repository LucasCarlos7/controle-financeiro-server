using ControleFinanceiro.Domain.Transacoes;

namespace ControleFinanceiro.Api.Contracts.Transacoes;

public sealed record CreateTransacaoRecorrenteRequest
{
    public required TipoTransacao Tipo { get; init; }
    public required decimal Valor { get; init; }
    public required DateOnly DataInicio { get; init; }
    public required string Descricao { get; init; }
    public Guid? ContaId { get; init; }
    public Guid? CartaoCreditoId { get; init; }
    public Guid? ContaDestinoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public IReadOnlyList<Guid> TagIds { get; init; } = [];

    public required TipoRecorrencia TipoRecorrencia { get; init; }
    public int? IntervaloMeses { get; init; }
    public int? TotalParcelas { get; init; }
    public DateOnly? DataFim { get; init; }
}
