using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;

namespace ControleFinanceiro.Application.Transacoes.Commands.DeleteTransacao;

public sealed class DeleteTransacaoCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeleteTransacaoCommand, Result>
{
    public async Task<Result> Handle(DeleteTransacaoCommand request, CancellationToken cancellationToken)
    {
        var transacao = await dbContext.Transacoes.FindAsync([request.Id], cancellationToken);
        if (transacao is null || transacao.UserId != request.UserId)
            return Result.Failure(TransacaoErrors.NaoEncontrada);

        dbContext.Transacoes.Remove(transacao);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
