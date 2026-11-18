using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;

namespace ControleFinanceiro.Application.Transacoes.Commands.CreateTransacao;

public sealed record CreateTransacaoCommand : IRequest<Result<Guid>>
{
    public required Guid UserId { get; init; }
    public required TipoTransacao Tipo { get; init; }
    public required decimal Valor { get; init; }
    public required DateOnly Data { get; init; }
    public required string Descricao { get; init; }
    public Guid? ContaId { get; init; }
    public Guid? CartaoCreditoId { get; init; }
    public Guid? ContaDestinoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public StatusTransacao Status { get; init; } = StatusTransacao.Pendente;
    public IReadOnlyList<Guid> TagIds { get; init; } = [];
}
