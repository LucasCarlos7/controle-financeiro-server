using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Categorias;
using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Categorias.Commands.UpdateCategoria;

public sealed class UpdateCategoriaCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<UpdateCategoriaCommand, Result>
{
    public async Task<Result> Handle(UpdateCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await dbContext.Categorias.FindAsync([request.Id], cancellationToken);
        if (categoria is null || categoria.UserId != request.UserId)
            return Result.Failure(CategoriaErrors.NaoEncontrada);

        var result = categoria.AtualizarDetalhes(request.Nome, request.Icone, request.Cor);
        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
