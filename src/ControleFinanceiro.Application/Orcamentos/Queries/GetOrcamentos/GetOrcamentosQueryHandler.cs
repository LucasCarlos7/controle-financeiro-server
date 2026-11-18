using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Orcamentos.Queries.GetOrcamentos;

public sealed class GetOrcamentosQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetOrcamentosQuery, Result<IReadOnlyList<OrcamentoResponse>>>
{
    public async Task<Result<IReadOnlyList<OrcamentoResponse>>> Handle(
        GetOrcamentosQuery request, CancellationToken cancellationToken)
    {
        var orcamentos = await dbContext.Orcamentos.AsNoTracking()
            .Where(o => o.UserId == request.UserId && o.Ano == request.Ano && o.Mes == request.Mes)
            .ToListAsync(cancellationToken);

        var consumido = await OrcamentoConsumoCalculator.CalcularConsumidoAsync(
            dbContext, orcamentos, cancellationToken);

        IReadOnlyList<OrcamentoResponse> response = orcamentos
            .Select(o => OrcamentoMapper.ToResponse(o, consumido[o.Id]))
            .ToList();

        return Result.Success(response);
    }
}
