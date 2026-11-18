using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Orcamentos;
using MediatR;

namespace ControleFinanceiro.Application.Orcamentos.Commands.UpdateOrcamento;

public sealed class UpdateOrcamentoCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<UpdateOrcamentoCommand, Result>
{
    public async Task<Result> Handle(UpdateOrcamentoCommand request, CancellationToken cancellationToken)
    {
        var orcamento = await dbContext.Orcamentos.FindAsync([request.Id], cancellationToken);
        if (orcamento is null || orcamento.UserId != request.UserId)
            return Result.Failure(OrcamentoErrors.NaoEncontrado);

        var result = orcamento.AtualizarLimite(request.LimiteValor);
        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
