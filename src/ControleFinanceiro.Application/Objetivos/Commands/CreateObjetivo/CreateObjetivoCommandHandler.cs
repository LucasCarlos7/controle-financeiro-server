using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Objetivos;
using MediatR;

namespace ControleFinanceiro.Application.Objetivos.Commands.CreateObjetivo;

public sealed class CreateObjetivoCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateObjetivoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateObjetivoCommand request, CancellationToken cancellationToken)
    {
        var result = Objetivo.Create(request.UserId, request.Nome, request.ValorAlvo, request.DataAlvo);
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        dbContext.Objetivos.Add(result.Value);
        await dbContext.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
