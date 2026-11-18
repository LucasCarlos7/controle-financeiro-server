using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Tags.Commands.CreateTag;

public sealed record CreateTagCommand : IRequest<Result<Guid>>
{
    public required Guid UserId { get; init; }
    public required string Nome { get; init; }
    public string? Icone { get; init; }
}
