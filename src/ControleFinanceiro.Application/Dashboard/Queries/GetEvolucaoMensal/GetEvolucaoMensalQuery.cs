using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Dashboard.Queries.GetEvolucaoMensal;

public sealed record GetEvolucaoMensalQuery : IRequest<Result<IReadOnlyList<EvolucaoMensalResponse>>>
{
    public required Guid UserId { get; init; }
    public required DateOnly DataInicio { get; init; }
    public required DateOnly DataFim { get; init; }
}
