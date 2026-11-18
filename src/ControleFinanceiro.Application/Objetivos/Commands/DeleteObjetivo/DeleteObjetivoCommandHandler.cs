using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Objetivos;
using MediatR;

namespace ControleFinanceiro.Application.Objetivos.Commands.DeleteObjetivo;

public sealed class DeleteObjetivoCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeleteObjetivoCommand, Result>
{
    public async Task<Result> Handle(DeleteObjetivoCommand request, CancellationToken cancellationToken)
    {
        var objetivo = await dbContext.Objetivos.FindAsync([request.Id], cancellationToken);
        if (objetivo is null || objetivo.UserId != request.UserId)
            return Result.Failure(ObjetivoErrors.NaoEncontrado);

        dbContext.Objetivos.Remove(objetivo);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
