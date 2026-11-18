using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Contas.Queries.GetCartaoCreditoById;

public sealed record GetCartaoCreditoByIdQuery : IRequest<Result<CartaoCreditoResponse>>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
}
