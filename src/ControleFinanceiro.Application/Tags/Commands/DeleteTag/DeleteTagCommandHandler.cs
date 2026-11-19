using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Tags;
using MediatR;

namespace ControleFinanceiro.Application.Tags.Commands.DeleteTag;

public sealed class DeleteTagCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeleteTagCommand, Result>
{
    public async Task<Result> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await dbContext.Tags.FindAsync([request.Id], cancellationToken);
        if (tag is null || tag.UserId != request.UserId)
            return Result.Failure(TagErrors.NaoEncontrada);

        if (tag.IsPadrao)
            return Result.Failure(TagErrors.ExclusaoDeTagPadrao);

        dbContext.Tags.Remove(tag);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
