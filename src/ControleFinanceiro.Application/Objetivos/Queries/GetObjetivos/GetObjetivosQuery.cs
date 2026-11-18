using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Objetivos.Queries.GetObjetivos;

public sealed record GetObjetivosQuery : IRequest<Result<IReadOnlyList<ObjetivoResponse>>>
{
    public required Guid UserId { get; init; }
}
