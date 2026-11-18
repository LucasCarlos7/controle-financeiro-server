using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Tags.Commands.DeleteTag;

public sealed record DeleteTagCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
}
