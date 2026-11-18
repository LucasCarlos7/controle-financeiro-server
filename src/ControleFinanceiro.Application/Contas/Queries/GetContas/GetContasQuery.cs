using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Contas.Queries.GetContas;

public sealed record GetContasQuery : IRequest<Result<IReadOnlyList<ContaResponse>>>
{
    public required Guid UserId { get; init; }
    public bool ApenasAtivas { get; init; }
}
