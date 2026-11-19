using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Categorias;
using ControleFinanceiro.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Categorias.Commands.CreateSubcategoria;

public sealed class CreateSubcategoriaCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateSubcategoriaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSubcategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await dbContext.Categorias
            .Include(c => c.Subcategorias)
            .FirstOrDefaultAsync(c => c.Id == request.CategoriaId && c.UserId == request.UserId, cancellationToken);

        if (categoria is null)
            return Result.Failure<Guid>(CategoriaErrors.NaoEncontrada);

        var result = categoria.AdicionarSubcategoria(request.Nome);
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        // EF Core não consegue inferir que entidades com GUID explícito adicionadas a coleções
        // com field access mode são novas (Added) — gerava UPDATE com 0 linhas ao invés de INSERT.
        dbContext.Subcategorias.Add(result.Value);

        await dbContext.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
