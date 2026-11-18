using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Tags.Queries.GetTags;

public sealed record GetTagsQuery : IRequest<Result<IReadOnlyList<TagResponse>>>
{
    public required Guid UserId { get; init; }
}
