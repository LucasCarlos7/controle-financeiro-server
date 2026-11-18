using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Tags;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Tags.Commands.CreateTag;

public sealed class CreateTagCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateTagCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var nomeNormalizado = request.Nome.Trim();

        var jaExiste = await dbContext.Tags
            .AnyAsync(t => t.UserId == request.UserId && t.Nome == nomeNormalizado, cancellationToken);

        if (jaExiste)
            return Result.Failure<Guid>(TagErrors.JaExiste);

        var result = Tag.Create(request.UserId, request.Nome, request.Icone);
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        dbContext.Tags.Add(result.Value);
        await dbContext.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
