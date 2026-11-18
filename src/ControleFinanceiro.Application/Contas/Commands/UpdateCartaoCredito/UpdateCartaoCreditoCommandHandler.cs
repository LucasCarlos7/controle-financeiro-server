using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Contas;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Contas.Commands.UpdateCartaoCredito;

public sealed class UpdateCartaoCreditoCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<UpdateCartaoCreditoCommand, Result>
{
    public async Task<Result> Handle(UpdateCartaoCreditoCommand request, CancellationToken cancellationToken)
    {
        var cartao = await dbContext.CartoesCredito.FindAsync([request.Id], cancellationToken);
        if (cartao is null || cartao.UserId != request.UserId)
            return Result.Failure(CartaoCreditoErrors.NaoEncontrado);

        if (request.ContaPagamentoId is { } contaPagamentoId)
        {
            var contaExiste = await dbContext.Contas
                .AnyAsync(c => c.Id == contaPagamentoId && c.UserId == request.UserId, cancellationToken);

            if (!contaExiste)
                return Result.Failure(CartaoCreditoErrors.ContaPagamentoNaoEncontrada);
        }

        var result = cartao.Atualizar(
            request.Nome, request.Limite, request.DiaFechamento, request.DiaVencimento, request.ContaPagamentoId, request.Icone);

        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
