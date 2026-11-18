using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Categorias;
using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Categorias.Commands.DeleteCategoria;

public sealed class DeleteCategoriaCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeleteCategoriaCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await dbContext.Categorias.FindAsync([request.Id], cancellationToken);
        if (categoria is null || categoria.UserId != request.UserId)
            return Result.Failure(CategoriaErrors.NaoEncontrada);

        dbContext.Categorias.Remove(categoria);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
