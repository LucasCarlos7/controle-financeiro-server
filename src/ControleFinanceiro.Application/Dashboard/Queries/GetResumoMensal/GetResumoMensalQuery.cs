using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Dashboard.Queries.GetResumoMensal;

public sealed record GetResumoMensalQuery : IRequest<Result<ResumoMensalResponse>>
{
    public required Guid UserId { get; init; }
    public required int Ano { get; init; }
    public required int Mes { get; init; }
}
