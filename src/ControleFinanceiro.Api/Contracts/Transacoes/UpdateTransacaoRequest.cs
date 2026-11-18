using ControleFinanceiro.Domain.Transacoes;

namespace ControleFinanceiro.Api.Contracts.Transacoes;

public sealed record UpdateTransacaoRequest
{
    public required decimal Valor { get; init; }
    public required DateOnly Data { get; init; }
    public required string Descricao { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public IReadOnlyList<Guid> TagIds { get; init; } = [];
    public EscopoEdicaoRecorrencia EscopoEdicao { get; init; } = EscopoEdicaoRecorrencia.SomenteEsta;
}
