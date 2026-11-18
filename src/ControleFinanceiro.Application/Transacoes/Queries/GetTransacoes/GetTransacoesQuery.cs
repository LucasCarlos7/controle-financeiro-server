using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;

namespace ControleFinanceiro.Application.Transacoes.Queries.GetTransacoes;

public sealed record GetTransacoesQuery : IRequest<Result<IReadOnlyList<TransacaoResponse>>>
{
    public required Guid UserId { get; init; }
    public DateOnly? DataInicio { get; init; }
    public DateOnly? DataFim { get; init; }
    public Guid? ContaId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? TagId { get; init; }
    public TipoTransacao? Tipo { get; init; }
    public StatusTransacao? Status { get; init; }
    public string? Busca { get; init; }
}
