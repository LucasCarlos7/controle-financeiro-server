using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Objetivos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Objetivos.Commands.CreateAporte;

public sealed class CreateAporteCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateAporteCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateAporteCommand request, CancellationToken cancellationToken)
    {
        var objetivoExiste = await dbContext.Objetivos
            .AnyAsync(o => o.Id == request.ObjetivoId && o.UserId == request.UserId, cancellationToken);

        if (!objetivoExiste)
            return Result.Failure<Guid>(ObjetivoErrors.NaoEncontrado);

        var result = Aporte.Create(request.ObjetivoId, request.Valor, request.Data);
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        dbContext.Aportes.Add(result.Value);
        await dbContext.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
