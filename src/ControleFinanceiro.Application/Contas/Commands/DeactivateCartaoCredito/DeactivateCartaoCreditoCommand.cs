using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Contas.Commands.DeactivateCartaoCredito;

public sealed record DeactivateCartaoCreditoCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
}
