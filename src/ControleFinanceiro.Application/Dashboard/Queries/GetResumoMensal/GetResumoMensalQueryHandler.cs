using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Dashboard.Queries.GetResumoMensal;

public sealed class GetResumoMensalQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetResumoMensalQuery, Result<ResumoMensalResponse>>
{
    public async Task<Result<ResumoMensalResponse>> Handle(
        GetResumoMensalQuery request, CancellationToken cancellationToken)
    {
        var transacoes = await dbContext.Transacoes.AsNoTracking()
            .Where(t => t.UserId == request.UserId
                && !t.Status.Equals(StatusTransacao.Cancelado)
                && t.Data.Year == request.Ano
                && t.Data.Month == request.Mes
                && (t.Tipo == TipoTransacao.Receita || t.Tipo == TipoTransacao.Despesa))
            .Select(t => new { t.Tipo, t.Valor, t.Status })
            .ToListAsync(cancellationToken);

        decimal receitasRecebidas = 0m, despesasPagas = 0m;
        decimal receitasPendentes = 0m, despesasPendentes = 0m;
        int qtdReceitasPendentes = 0, qtdDespesasPendentes = 0;

        foreach (var t in transacoes)
        {
            var realizado = t.Status == StatusTransacao.Pago || t.Status == StatusTransacao.Recebido;
            var pendente = t.Status == StatusTransacao.Pendente;

            if (t.Tipo == TipoTransacao.Receita)
            {
                if (realizado) receitasRecebidas += t.Valor;
                else if (pendente) { receitasPendentes += t.Valor; qtdReceitasPendentes++; }
            }
            else
            {
                if (realizado) despesasPagas += t.Valor;
                else if (pendente) { despesasPendentes += t.Valor; qtdDespesasPendentes++; }
            }
        }

        var saldoRealizado = receitasRecebidas - despesasPagas;
        var receitasPrevistas = receitasRecebidas + receitasPendentes;
        var despesasPrevistas = despesasPagas + despesasPendentes;
        var saldoPrevisto = receitasPrevistas - despesasPrevistas;

        return Result.Success(new ResumoMensalResponse
        {
            Ano = request.Ano,
            Mes = request.Mes,
            ReceitasRecebidas = receitasRecebidas,
            DespesasPagas = despesasPagas,
            SaldoRealizado = saldoRealizado,
            ReceitasPendentes = receitasPendentes,
            DespesasPendentes = despesasPendentes,
            ReceitasPrevistas = receitasPrevistas,
            DespesasPrevistas = despesasPrevistas,
            SaldoPrevisto = saldoPrevisto,
            QtdReceitasPendentes = qtdReceitasPendentes,
            QtdDespesasPendentes = qtdDespesasPendentes,
        });
    }
}
