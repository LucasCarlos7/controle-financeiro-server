using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Dashboard.Queries.GetGastosPorCategoria;

public sealed class GetGastosPorCategoriaQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetGastosPorCategoriaQuery, Result<IReadOnlyList<GastoPorCategoriaResponse>>>
{
    public async Task<Result<IReadOnlyList<GastoPorCategoriaResponse>>> Handle(
        GetGastosPorCategoriaQuery request, CancellationToken cancellationToken)
    {
        var despesas = await dbContext.Transacoes.AsNoTracking()
            .Where(t => t.UserId == request.UserId
                && t.Status == StatusTransacao.Pago
                && t.Tipo == TipoTransacao.Despesa
                && t.Data.Year == request.Ano
                && t.Data.Month == request.Mes)
            .Select(t => new { t.CategoriaId, t.Valor })
            .ToListAsync(cancellationToken);

        var categoriaIds = despesas.Where(d => d.CategoriaId is not null)
            .Select(d => d.CategoriaId!.Value)
            .Distinct()
            .ToList();

        var categorias = await dbContext.Categorias.AsNoTracking()
            .Where(c => categoriaIds.Contains(c.Id))
            .Select(c => new { c.Id, c.Nome })
            .ToDictionaryAsync(c => c.Id, c => c.Nome, cancellationToken);

        IReadOnlyList<GastoPorCategoriaResponse> response = despesas
            .GroupBy(d => d.CategoriaId)
            .Select(g => new GastoPorCategoriaResponse
            {
                CategoriaId = g.Key,
                CategoriaNome = g.Key is { } categoriaId && categorias.TryGetValue(categoriaId, out var nome)
                    ? nome
                    : "Sem categoria",
                Valor = g.Sum(d => d.Valor),
            })
            .OrderByDescending(g => g.Valor)
            .ToList();

        return Result.Success(response);
    }
}
