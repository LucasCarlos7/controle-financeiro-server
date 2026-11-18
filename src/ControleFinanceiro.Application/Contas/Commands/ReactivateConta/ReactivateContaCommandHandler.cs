using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Contas;
using MediatR;

namespace ControleFinanceiro.Application.Contas.Commands.ReactivateConta;

public sealed class ReactivateContaCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<ReactivateContaCommand, Result>
{
    public async Task<Result> Handle(ReactivateContaCommand request, CancellationToken cancellationToken)
    {
        var conta = await dbContext.Contas.FindAsync([request.Id], cancellationToken);
        if (conta is null || conta.UserId != request.UserId)
            return Result.Failure(ContaErrors.NaoEncontrada);

        conta.Reativar();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
