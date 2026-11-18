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

        var saldoAnterior = await dbContext.Transacoes.AsNoTracking()
            .Where(t => t.UserId == request.UserId
                && t.Status == StatusTransacao.Pago
                && t.Data < request.DataInicio
                && (t.Tipo == TipoTransacao.Receita || t.Tipo == TipoTransacao.Despesa))
            .SumAsync(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor, cancellationToken);

        var transacoesNoIntervalo = await dbContext.Transacoes.AsNoTracking()
            .Where(t => t.UserId == request.UserId
                && t.Status == StatusTransacao.Pago
                && t.Data >= request.DataInicio
                && t.Data <= request.DataFim
                && (t.Tipo == TipoTransacao.Receita || t.Tipo == TipoTransacao.Despesa))
            .Select(t => new { t.Tipo, t.Valor, t.Data.Year, t.Data.Month })
            .ToListAsync(cancellationToken);

        var saldoAcumulado = saldoInicialContas + saldoAnterior;
        var response = new List<EvolucaoMensalResponse>();

        var mesAtual = new DateOnly(request.DataInicio.Year, request.DataInicio.Month, 1);
        var mesFinal = new DateOnly(request.DataFim.Year, request.DataFim.Month, 1);

        while (mesAtual <= mesFinal)
        {
            var doMes = transacoesNoIntervalo
                .Where(t => t.Year == mesAtual.Year && t.Month == mesAtual.Month)
                .ToList();

            var receitas = doMes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor);
            var despesas = doMes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor);

            saldoAcumulado += receitas - despesas;

            response.Add(new EvolucaoMensalResponse
            {
                Ano = mesAtual.Year,
                Mes = mesAtual.Month,
                Receitas = receitas,
                Despesas = despesas,
                SaldoAcumulado = saldoAcumulado,
            });

            mesAtual = mesAtual.AddMonths(1);
        }

        return Result.Success<IReadOnlyList<EvolucaoMensalResponse>>(response);
    }
}
