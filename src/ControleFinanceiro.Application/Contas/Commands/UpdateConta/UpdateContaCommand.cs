using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Contas.Commands.UpdateConta;

public sealed record UpdateContaCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
    public required string Nome { get; init; }
    public string? Icone { get; init; }
}
