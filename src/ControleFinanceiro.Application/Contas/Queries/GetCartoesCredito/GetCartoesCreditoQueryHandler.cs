using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Contas.Queries.GetCartoesCredito;

public sealed class GetCartoesCreditoQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetCartoesCreditoQuery, Result<IReadOnlyList<CartaoCreditoResponse>>>
{
    public async Task<Result<IReadOnlyList<CartaoCreditoResponse>>> Handle(
        GetCartoesCreditoQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.CartoesCredito.AsNoTracking().Where(c => c.UserId == request.UserId);

        if (request.ApenasAtivos)
            query = query.Where(c => c.Ativo);

        var cartoes = await query
            .Select(c => new
            {
                c.Id, c.Nome, c.Limite, c.DiaFechamento, c.DiaVencimento, c.ContaPagamentoId, c.Icone, c.Ativo,
            })
            .ToListAsync(cancellationToken);

        var consumido = await LimiteCalculator.CalcularConsumidoAsync(
            dbContext, cartoes.Select(c => c.Id).ToList(), cancellationToken);

        IReadOnlyList<CartaoCreditoResponse> response = cartoes
            .Select(c => new CartaoCreditoResponse
            {
                Id = c.Id,
                Nome = c.Nome,
                Limite = c.Limite,
                LimiteDisponivel = c.Limite - consumido[c.Id],
                DiaFechamento = c.DiaFechamento,
                DiaVencimento = c.DiaVencimento,
                ContaPagamentoId = c.ContaPagamentoId,
                Icone = c.Icone,
                Ativo = c.Ativo,
            })
            .ToList();

        return Result.Success(response);
    }
}
