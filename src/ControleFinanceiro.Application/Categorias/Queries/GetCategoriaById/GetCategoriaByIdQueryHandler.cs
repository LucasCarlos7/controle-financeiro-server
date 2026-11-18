using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Categorias;
using ControleFinanceiro.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Categorias.Queries.GetCategoriaById;

public sealed class GetCategoriaByIdQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetCategoriaByIdQuery, Result<CategoriaResponse>>
{
    public async Task<Result<CategoriaResponse>> Handle(
        GetCategoriaByIdQuery request, CancellationToken cancellationToken)
    {
        var categoria = await dbContext.Categorias.AsNoTracking()
            .Include(c => c.Subcategorias)
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == request.UserId, cancellationToken);

        if (categoria is null)
            return Result.Failure<CategoriaResponse>(CategoriaErrors.NaoEncontrada);

        return Result.Success(CategoriaMapper.ToResponse(categoria));
    }
}
