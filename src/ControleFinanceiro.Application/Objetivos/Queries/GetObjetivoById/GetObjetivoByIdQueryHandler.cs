using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Objetivos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Objetivos.Queries.GetObjetivoById;

public sealed class GetObjetivoByIdQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetObjetivoByIdQuery, Result<ObjetivoResponse>>
{
    public async Task<Result<ObjetivoResponse>> Handle(
        GetObjetivoByIdQuery request, CancellationToken cancellationToken)
    {
        var objetivo = await dbContext.Objetivos.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.Id && o.UserId == request.UserId, cancellationToken);

        if (objetivo is null)
            return Result.Failure<ObjetivoResponse>(ObjetivoErrors.NaoEncontrado);

        var aportes = await dbContext.Aportes.AsNoTracking()
            .Where(a => a.ObjetivoId == objetivo.Id)
            .ToListAsync(cancellationToken);

        return Result.Success(ObjetivoMapper.ToResponse(objetivo, aportes));
    }
}
