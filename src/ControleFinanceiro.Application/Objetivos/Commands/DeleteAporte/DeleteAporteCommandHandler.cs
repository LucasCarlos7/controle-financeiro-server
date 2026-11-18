using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Objetivos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Objetivos.Commands.DeleteAporte;

public sealed class DeleteAporteCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeleteAporteCommand, Result>
{
    public async Task<Result> Handle(DeleteAporteCommand request, CancellationToken cancellationToken)
    {
        var aporte = await dbContext.Aportes.FindAsync([request.Id], cancellationToken);
        if (aporte is null || aporte.ObjetivoId != request.ObjetivoId)
            return Result.Failure(ObjetivoErrors.AporteNaoEncontrado);

        // Aporte não guarda o usuário: o dono é o do objetivo.
        var objetivoDoUsuario = await dbContext.Objetivos
            .AnyAsync(o => o.Id == aporte.ObjetivoId && o.UserId == request.UserId, cancellationToken);

        if (!objetivoDoUsuario)
            return Result.Failure(ObjetivoErrors.AporteNaoEncontrado);

        dbContext.Aportes.Remove(aporte);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
