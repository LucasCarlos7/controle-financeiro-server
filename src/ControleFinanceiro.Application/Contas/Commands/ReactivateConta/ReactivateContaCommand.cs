using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Contas.Commands.ReactivateConta;

public sealed record ReactivateContaCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
}
