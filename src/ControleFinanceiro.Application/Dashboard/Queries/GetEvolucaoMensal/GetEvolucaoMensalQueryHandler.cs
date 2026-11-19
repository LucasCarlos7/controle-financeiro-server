using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Dashboard.Queries.GetEvolucaoMensal;

public sealed class GetEvolucaoMensalQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetEvolucaoMensalQuery, Result<IReadOnlyList<EvolucaoMensalResponse>>>
{
    public async Task<Result<IReadOnlyList<EvolucaoMensalResponse>>> Handle(
        GetEvolucaoMensalQuery request, CancellationToken cancellationToken)
    {
        var saldoInicialContas = await dbContext.Contas.AsNoTracking()
            .Where(c => c.UserId == request.UserId)
            .SumAsync(c => c.SaldoInicial, cancellationToken);

        // Saldo realizado antes do período (apenas efetivados)
        var saldoAnteriorRealizado = await dbContext.Transacoes.AsNoTracking()
            .Where(t => t.UserId == request.UserId
                && (t.Status == StatusTransacao.Pago || t.Status == StatusTransacao.Recebido)
                && t.Data < request.DataInicio
                && (t.Tipo == TipoTransacao.Receita || t.Tipo == TipoTransacao.Despesa))
            .SumAsync(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor, cancellationToken);

        // Saldo previsto antes do período (efetivados + pendentes)
        var saldoAnteriorPrevisto = await dbContext.Transacoes.AsNoTracking()
            .Where(t => t.UserId == request.UserId
                && t.Status != StatusTransacao.Cancelado
                && t.Data < request.DataInicio
                && (t.Tipo == TipoTransacao.Receita || t.Tipo == TipoTransacao.Despesa))
            .SumAsync(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor, cancellationToken);

        var transacoesNoIntervalo = await dbContext.Transacoes.AsNoTracking()
            .Where(t => t.UserId == request.UserId
                && t.Status != StatusTransacao.Cancelado
                && t.Data >= request.DataInicio
                && t.Data <= request.DataFim
                && (t.Tipo == TipoTransacao.Receita || t.Tipo == TipoTransacao.Despesa))
            .Select(t => new { t.Tipo, t.Valor, t.Status, t.Data.Year, t.Data.Month })
            .ToListAsync(cancellationToken);

        var saldoAcumuladoRealizado = saldoInicialContas + saldoAnteriorRealizado;
        var saldoAcumuladoPrevisto = saldoInicialContas + saldoAnteriorPrevisto;
        var response = new List<EvolucaoMensalResponse>();

        var mesAtual = new DateOnly(request.DataInicio.Year, request.DataInicio.Month, 1);
        var mesFinal = new DateOnly(request.DataFim.Year, request.DataFim.Month, 1);

        while (mesAtual <= mesFinal)
        {
            var doMes = transacoesNoIntervalo
                .Where(t => t.Year == mesAtual.Year && t.Month == mesAtual.Month)
                .ToList();

            var receitasRealizadas = doMes
                .Where(t => t.Tipo == TipoTransacao.Receita
                    && (t.Status == StatusTransacao.Pago || t.Status == StatusTransacao.Recebido))
                .Sum(t => t.Valor);

            var despesasRealizadas = doMes
                .Where(t => t.Tipo == TipoTransacao.Despesa
                    && (t.Status == StatusTransacao.Pago || t.Status == StatusTransacao.Recebido))
                .Sum(t => t.Valor);

            var receitasPrevistas = doMes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor);
            var despesasPrevistas = doMes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor);

            var saldoRealizado = receitasRealizadas - despesasRealizadas;
            var saldoPrevisto = receitasPrevistas - despesasPrevistas;

            saldoAcumuladoRealizado += saldoRealizado;
            saldoAcumuladoPrevisto += saldoPrevisto;

            response.Add(new EvolucaoMensalResponse
            {
                Ano = mesAtual.Year,
                Mes = mesAtual.Month,
                ReceitasRealizadas = receitasRealizadas,
                DespesasRealizadas = despesasRealizadas,
                SaldoRealizado = saldoRealizado,
                SaldoAcumuladoRealizado = saldoAcumuladoRealizado,
                ReceitasPrevistas = receitasPrevistas,
                DespesasPrevistas = despesasPrevistas,
                SaldoPrevisto = saldoPrevisto,
                SaldoAcumuladoPrevisto = saldoAcumuladoPrevisto,
            });

            mesAtual = mesAtual.AddMonths(1);
        }

        return Result.Success<IReadOnlyList<EvolucaoMensalResponse>>(response);
    }
}
