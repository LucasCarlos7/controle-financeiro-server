using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Contas.Queries.GetContaById;

public sealed record GetContaByIdQuery : IRequest<Result<ContaResponse>>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
}
