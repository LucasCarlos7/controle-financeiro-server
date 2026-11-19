using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Transacoes;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Contas;

/// <summary>
/// Calcula o saldo atual de contas a partir das transações pagas, sem persistir saldo em coluna própria.
/// </summary>
internal static class SaldoCalculator
{
    public static async Task<Dictionary<Guid, decimal>> CalcularDeltasAsync(
        IApplicationDbContext dbContext,
        IReadOnlyCollection<Guid> contaIds,
        CancellationToken cancellationToken)
    {
        var deltas = contaIds.ToDictionary(id => id, _ => 0m);

        if (contaIds.Count == 0)
            return deltas;

        var transacoes = await dbContext.Transacoes
            .Where(t => (t.Status == StatusTransacao.Pago || t.Status == StatusTransacao.Recebido)
                && ((t.ContaId != null && contaIds.Contains(t.ContaId.Value))
                    || (t.ContaDestinoId != null && contaIds.Contains(t.ContaDestinoId.Value))))
            .Select(t => new { t.ContaId, t.ContaDestinoId, t.Tipo, t.Valor })
            .ToListAsync(cancellationToken);

        foreach (var transacao in transacoes)
        {
            if (transacao.ContaId is { } contaId && deltas.ContainsKey(contaId))
            {
                deltas[contaId] += transacao.Tipo switch
                {
                    TipoTransacao.Receita => transacao.Valor,
                    TipoTransacao.Despesa => -transacao.Valor,
                    TipoTransacao.Transferencia => -transacao.Valor,
                    _ => 0m,
                };
            }

            if (transacao.ContaDestinoId is { } contaDestinoId && deltas.ContainsKey(contaDestinoId))
                deltas[contaDestinoId] += transacao.Valor;
        }

        return deltas;
    }
}
