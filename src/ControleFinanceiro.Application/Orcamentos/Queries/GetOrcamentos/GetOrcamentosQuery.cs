using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Orcamentos.Queries.GetOrcamentos;

public sealed record GetOrcamentosQuery : IRequest<Result<IReadOnlyList<OrcamentoResponse>>>
{
    public required Guid UserId { get; init; }
    public required int Ano { get; init; }
    public required int Mes { get; init; }
}
