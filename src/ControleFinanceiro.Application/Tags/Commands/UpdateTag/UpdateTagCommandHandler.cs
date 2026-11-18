using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Tags;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Tags.Commands.UpdateTag;

public sealed class UpdateTagCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<UpdateTagCommand, Result>
{
    public async Task<Result> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await dbContext.Tags.FindAsync([request.Id], cancellationToken);
        if (tag is null || tag.UserId != request.UserId)
            return Result.Failure(TagErrors.NaoEncontrada);

        var nomeNormalizado = request.Nome.Trim();

        var nomeEmUso = await dbContext.Tags
            .AnyAsync(t => t.UserId == request.UserId && t.Id != request.Id && t.Nome == nomeNormalizado, cancellationToken);

        if (nomeEmUso)
            return Result.Failure(TagErrors.JaExiste);

        var result = tag.AtualizarDetalhes(request.Nome, request.Icone);
        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
