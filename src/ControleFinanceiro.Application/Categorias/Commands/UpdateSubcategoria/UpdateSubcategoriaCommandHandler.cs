using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Categorias;
using ControleFinanceiro.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Categorias.Commands.UpdateSubcategoria;

public sealed class UpdateSubcategoriaCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<UpdateSubcategoriaCommand, Result>
{
    public async Task<Result> Handle(UpdateSubcategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await dbContext.Categorias
            .Include(c => c.Subcategorias)
            .FirstOrDefaultAsync(c => c.Id == request.CategoriaId && c.UserId == request.UserId, cancellationToken);

        if (categoria is null)
            return Result.Failure(CategoriaErrors.NaoEncontrada);

        var result = categoria.AtualizarSubcategoria(request.SubcategoriaId, request.Nome);
        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
