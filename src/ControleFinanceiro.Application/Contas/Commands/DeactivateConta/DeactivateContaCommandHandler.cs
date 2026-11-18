using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Contas;
using MediatR;

namespace ControleFinanceiro.Application.Contas.Commands.DeactivateConta;

public sealed class DeactivateContaCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeactivateContaCommand, Result>
{
    public async Task<Result> Handle(DeactivateContaCommand request, CancellationToken cancellationToken)
    {
        var conta = await dbContext.Contas.FindAsync([request.Id], cancellationToken);
        if (conta is null || conta.UserId != request.UserId)
            return Result.Failure(ContaErrors.NaoEncontrada);

        conta.Desativar();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
