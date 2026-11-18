using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Categorias.Queries.GetCategorias;

public sealed class GetCategoriasQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetCategoriasQuery, Result<IReadOnlyList<CategoriaResponse>>>
{
    public async Task<Result<IReadOnlyList<CategoriaResponse>>> Handle(
        GetCategoriasQuery request, CancellationToken cancellationToken)
    {
        var categorias = await dbContext.Categorias.AsNoTracking()
            .Include(c => c.Subcategorias)
            .Where(c => c.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        IReadOnlyList<CategoriaResponse> response = categorias.Select(CategoriaMapper.ToResponse).ToList();

        return Result.Success(response);
    }
}
