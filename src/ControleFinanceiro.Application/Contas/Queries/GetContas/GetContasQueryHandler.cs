using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Contas.Queries.GetContas;

public sealed class GetContasQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetContasQuery, Result<IReadOnlyList<ContaResponse>>>
{
    public async Task<Result<IReadOnlyList<ContaResponse>>> Handle(
        GetContasQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Contas.AsNoTracking().Where(c => c.UserId == request.UserId);

        if (request.ApenasAtivas)
            query = query.Where(c => c.Ativo);

        var contas = await query
            .Select(c => new { c.Id, c.Nome, c.Tipo, c.SaldoInicial, c.Icone, c.Ativo })
            .ToListAsync(cancellationToken);

        var deltas = await SaldoCalculator.CalcularDeltasAsync(
            dbContext, contas.Select(c => c.Id).ToList(), cancellationToken);

        IReadOnlyList<ContaResponse> response = contas
            .Select(c => new ContaResponse
            {
                Id = c.Id,
                Nome = c.Nome,
                Tipo = c.Tipo,
                SaldoInicial = c.SaldoInicial,
                SaldoAtual = c.SaldoInicial + deltas[c.Id],
                Icone = c.Icone,
                Ativo = c.Ativo,
            })
            .ToList();

        return Result.Success(response);
    }
}
