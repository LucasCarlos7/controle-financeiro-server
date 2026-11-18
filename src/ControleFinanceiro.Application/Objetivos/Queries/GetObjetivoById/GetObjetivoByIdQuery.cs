using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Objetivos.Queries.GetObjetivoById;

public sealed record GetObjetivoByIdQuery : IRequest<Result<ObjetivoResponse>>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
}
