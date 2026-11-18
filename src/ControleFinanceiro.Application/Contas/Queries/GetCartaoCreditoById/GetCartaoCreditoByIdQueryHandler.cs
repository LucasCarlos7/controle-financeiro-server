using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Contas;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Contas.Queries.GetCartaoCreditoById;

public sealed class GetCartaoCreditoByIdQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetCartaoCreditoByIdQuery, Result<CartaoCreditoResponse>>
{
    public async Task<Result<CartaoCreditoResponse>> Handle(
        GetCartaoCreditoByIdQuery request, CancellationToken cancellationToken)
    {
        var cartao = await dbContext.CartoesCredito.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == request.UserId, cancellationToken);

        if (cartao is null)
            return Result.Failure<CartaoCreditoResponse>(CartaoCreditoErrors.NaoEncontrado);

        var consumido = await LimiteCalculator.CalcularConsumidoAsync(dbContext, [cartao.Id], cancellationToken);

        return new CartaoCreditoResponse
        {
            Id = cartao.Id,
            Nome = cartao.Nome,
            Limite = cartao.Limite,
            LimiteDisponivel = cartao.Limite - consumido[cartao.Id],
            DiaFechamento = cartao.DiaFechamento,
            DiaVencimento = cartao.DiaVencimento,
            ContaPagamentoId = cartao.ContaPagamentoId,
            Icone = cartao.Icone,
            Ativo = cartao.Ativo,
        };
    }
}
