using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Orcamentos;
using ControleFinanceiro.Domain.Transacoes;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Orcamentos;

/// <param name="Total">Pagas e Pendentes: o orçamento funciona como previsão do mês.</param>
/// <param name="Pendente">Parte do total ainda não paga, exibida separadamente para explicar a diferença com o painel.</param>
internal readonly record struct ConsumoOrcamento(decimal Total, decimal Pendente);

/// <summary>
/// Calcula o valor consumido de cada orçamento a partir das despesas lançadas na categoria
/// dentro do mês de referência (Pagas ou Pendentes — ambas contam como gasto planejado).
/// </summary>
internal static class OrcamentoConsumoCalculator
{
    public static async Task<Dictionary<Guid, ConsumoOrcamento>> CalcularConsumidoAsync(
        IApplicationDbContext dbContext,
        IReadOnlyCollection<Orcamento> orcamentos,
        CancellationToken cancellationToken)
    {
        var consumido = orcamentos.ToDictionary(o => o.Id, _ => new ConsumoOrcamento(0m, 0m));

        if (orcamentos.Count == 0)
            return consumido;

        var categoriaIds = orcamentos.Select(o => o.CategoriaId).Distinct().ToList();

        var despesas = await dbContext.Transacoes
            .Where(t => t.Tipo == TipoTransacao.Despesa
                && t.CategoriaId != null
                && categoriaIds.Contains(t.CategoriaId.Value))
            .Select(t => new { t.CategoriaId, t.Data, t.Valor, t.Status })
            .ToListAsync(cancellationToken);

        foreach (var orcamento in orcamentos)
        {
            var doMes = despesas
                .Where(d => d.CategoriaId == orcamento.CategoriaId
                    && d.Data.Year == orcamento.Ano
                    && d.Data.Month == orcamento.Mes)
                .ToList();

            consumido[orcamento.Id] = new ConsumoOrcamento(
                doMes.Sum(d => d.Valor),
                doMes.Where(d => d.Status == StatusTransacao.Pendente).Sum(d => d.Valor));
        }

        return consumido;
    }
}
