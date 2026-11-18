using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Categorias;
using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Categorias.Commands.CreateCategoria;

public sealed class CreateCategoriaCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateCategoriaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCategoriaCommand request, CancellationToken cancellationToken)
    {
        var result = Categoria.Create(request.UserId, request.Nome, request.Tipo, request.Icone, request.Cor);
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        dbContext.Categorias.Add(result.Value);
        await dbContext.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
