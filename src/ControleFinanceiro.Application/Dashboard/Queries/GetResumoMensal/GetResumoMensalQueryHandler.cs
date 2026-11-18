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
                && t.Status == StatusTransacao.Pago
                && t.Data.Year == request.Ano
                && t.Data.Month == request.Mes
                && (t.Tipo == TipoTransacao.Receita || t.Tipo == TipoTransacao.Despesa))
            .Select(t => new { t.Tipo, t.Valor })
            .ToListAsync(cancellationToken);

        var receitas = transacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor);
        var despesas = transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor);

        return Result.Success(new ResumoMensalResponse
        {
            Ano = request.Ano,
            Mes = request.Mes,
            Receitas = receitas,
            Despesas = despesas,
            Saldo = receitas - despesas,
        });
    }
}
