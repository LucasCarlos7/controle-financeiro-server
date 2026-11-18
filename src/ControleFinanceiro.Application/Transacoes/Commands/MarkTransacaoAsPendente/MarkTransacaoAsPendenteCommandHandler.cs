using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;

namespace ControleFinanceiro.Application.Transacoes.Commands.MarkTransacaoAsPendente;

public sealed class MarkTransacaoAsPendenteCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<MarkTransacaoAsPendenteCommand, Result>
{
    public async Task<Result> Handle(MarkTransacaoAsPendenteCommand request, CancellationToken cancellationToken)
    {
        var transacao = await dbContext.Transacoes.FindAsync([request.Id], cancellationToken);
        if (transacao is null || transacao.UserId != request.UserId)
            return Result.Failure(TransacaoErrors.NaoEncontrada);

        transacao.MarcarComoPendente();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
