using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;

namespace ControleFinanceiro.Application.Transacoes.Commands.UpdateTransacao;

public sealed record UpdateTransacaoCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
    public required decimal Valor { get; init; }
    public required DateOnly Data { get; init; }
    public required string Descricao { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public IReadOnlyList<Guid> TagIds { get; init; } = [];
    public EscopoEdicaoRecorrencia EscopoEdicao { get; init; } = EscopoEdicaoRecorrencia.SomenteEsta;
}
