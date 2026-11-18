using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Orcamentos;
using MediatR;

namespace ControleFinanceiro.Application.Orcamentos.Commands.DeleteOrcamento;

public sealed class DeleteOrcamentoCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeleteOrcamentoCommand, Result>
{
    public async Task<Result> Handle(DeleteOrcamentoCommand request, CancellationToken cancellationToken)
    {
        var orcamento = await dbContext.Orcamentos.FindAsync([request.Id], cancellationToken);
        if (orcamento is null || orcamento.UserId != request.UserId)
            return Result.Failure(OrcamentoErrors.NaoEncontrado);

        dbContext.Orcamentos.Remove(orcamento);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
