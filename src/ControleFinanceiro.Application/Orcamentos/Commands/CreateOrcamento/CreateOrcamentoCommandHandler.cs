using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Categorias;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Orcamentos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Orcamentos.Commands.CreateOrcamento;

public sealed class CreateOrcamentoCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateOrcamentoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrcamentoCommand request, CancellationToken cancellationToken)
    {
        var categoriaExiste = await dbContext.Categorias
            .AnyAsync(c => c.Id == request.CategoriaId && c.UserId == request.UserId, cancellationToken);

        if (!categoriaExiste)
            return Result.Failure<Guid>(CategoriaErrors.NaoEncontrada);

        var jaExiste = await dbContext.Orcamentos.AnyAsync(
            o => o.UserId == request.UserId
                && o.CategoriaId == request.CategoriaId
                && o.Ano == request.Ano
                && o.Mes == request.Mes,
            cancellationToken);

        if (jaExiste)
            return Result.Failure<Guid>(OrcamentoErrors.JaExiste);

        var result = Orcamento.Create(
            request.UserId, request.CategoriaId, request.Ano, request.Mes, request.LimiteValor);

        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        dbContext.Orcamentos.Add(result.Value);
        await dbContext.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
