using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Transacoes;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Contas;

/// <summary>
/// Calcula o limite disponível de cartões a partir das despesas lançadas neles.
/// Sem modelar fatura, a despesa Pendente representa compra ainda não paga e ocupa limite;
/// marcá-la como Paga equivale a quitá-la na fatura e libera o limite.
/// </summary>
internal static class LimiteCalculator
{
    public static async Task<Dictionary<Guid, decimal>> CalcularConsumidoAsync(
        IApplicationDbContext dbContext,
        IReadOnlyCollection<Guid> cartaoIds,
        CancellationToken cancellationToken)
    {
        var consumido = cartaoIds.ToDictionary(id => id, _ => 0m);

        if (cartaoIds.Count == 0)
            return consumido;

        var despesas = await dbContext.Transacoes
            .Where(t => t.Tipo == TipoTransacao.Despesa
                && t.Status == StatusTransacao.Pendente
                && t.CartaoCreditoId != null
                && cartaoIds.Contains(t.CartaoCreditoId.Value))
            .Select(t => new { t.CartaoCreditoId, t.Valor })
            .ToListAsync(cancellationToken);

        foreach (var despesa in despesas)
            consumido[despesa.CartaoCreditoId!.Value] += despesa.Valor;

        return consumido;
    }
}
