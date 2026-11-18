using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Objetivos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Objetivos.Queries.GetObjetivos;

public sealed class GetObjetivosQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetObjetivosQuery, Result<IReadOnlyList<ObjetivoResponse>>>
{
    public async Task<Result<IReadOnlyList<ObjetivoResponse>>> Handle(
        GetObjetivosQuery request, CancellationToken cancellationToken)
    {
        var objetivos = await dbContext.Objetivos.AsNoTracking()
            .Where(o => o.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var objetivoIds = objetivos.Select(o => o.Id).ToList();

        var aportes = await dbContext.Aportes.AsNoTracking()
            .Where(a => objetivoIds.Contains(a.ObjetivoId))
            .ToListAsync(cancellationToken);

        IReadOnlyList<ObjetivoResponse> response = objetivos
            .Select(o => ObjetivoMapper.ToResponse(o, aportes.Where(a => a.ObjetivoId == o.Id).ToList()))
            .ToList();

        return Result.Success(response);
    }
}
