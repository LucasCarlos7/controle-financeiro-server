using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;

namespace ControleFinanceiro.Application.Transacoes.Commands.MarkTransacaoAsPago;

public sealed class MarkTransacaoAsPagoCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<MarkTransacaoAsPagoCommand, Result>
{
    public async Task<Result> Handle(MarkTransacaoAsPagoCommand request, CancellationToken cancellationToken)
    {
        var transacao = await dbContext.Transacoes.FindAsync([request.Id], cancellationToken);
        if (transacao is null || transacao.UserId != request.UserId)
            return Result.Failure(TransacaoErrors.NaoEncontrada);

        var efetivadaResult = transacao.MarcarComoEfetivada();
        if (efetivadaResult.IsFailure)
            return efetivadaResult;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
