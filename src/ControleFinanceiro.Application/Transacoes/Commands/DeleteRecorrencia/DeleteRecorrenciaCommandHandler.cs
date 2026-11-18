using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Transacoes.Commands.DeleteRecorrencia;

public sealed class DeleteRecorrenciaCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeleteRecorrenciaCommand, Result>
{
    public async Task<Result> Handle(DeleteRecorrenciaCommand request, CancellationToken cancellationToken)
    {
        var recorrencia = await dbContext.Recorrencias.FindAsync([request.Id], cancellationToken);
        if (recorrencia is null || recorrencia.UserId != request.UserId)
            return Result.Failure(RecorrenciaErrors.NaoEncontrada);

        var transacoesPendentes = await dbContext.Transacoes
            .Where(t => t.RecorrenciaId == request.Id && t.Status == StatusTransacao.Pendente)
            .ToListAsync(cancellationToken);

        dbContext.Transacoes.RemoveRange(transacoesPendentes);
        dbContext.Recorrencias.Remove(recorrencia);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
