using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Dashboard.Queries.GetGastosPorCategoria;

public sealed record GetGastosPorCategoriaQuery : IRequest<Result<IReadOnlyList<GastoPorCategoriaResponse>>>
{
    public required Guid UserId { get; init; }
    public required int Ano { get; init; }
    public required int Mes { get; init; }
}
