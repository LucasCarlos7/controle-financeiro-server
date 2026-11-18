using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Objetivos;
using MediatR;

namespace ControleFinanceiro.Application.Objetivos.Commands.UpdateObjetivo;

public sealed class UpdateObjetivoCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<UpdateObjetivoCommand, Result>
{
    public async Task<Result> Handle(UpdateObjetivoCommand request, CancellationToken cancellationToken)
    {
        var objetivo = await dbContext.Objetivos.FindAsync([request.Id], cancellationToken);
        if (objetivo is null || objetivo.UserId != request.UserId)
            return Result.Failure(ObjetivoErrors.NaoEncontrado);

        var result = objetivo.Atualizar(request.Nome, request.ValorAlvo, request.DataAlvo);
        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
