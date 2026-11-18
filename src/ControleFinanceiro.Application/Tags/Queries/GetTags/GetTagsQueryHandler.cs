using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Tags.Queries.GetTags;

public sealed class GetTagsQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetTagsQuery, Result<IReadOnlyList<TagResponse>>>
{
    public async Task<Result<IReadOnlyList<TagResponse>>> Handle(
        GetTagsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<TagResponse> tags = await dbContext.Tags.AsNoTracking()
            .Where(t => t.UserId == request.UserId)
            .Select(t => new TagResponse { Id = t.Id, Nome = t.Nome, Icone = t.Icone })
            .ToListAsync(cancellationToken);

        return Result.Success(tags);
    }
}
