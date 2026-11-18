using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Contas;
using MediatR;

namespace ControleFinanceiro.Application.Contas.Commands.UpdateConta;

public sealed class UpdateContaCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<UpdateContaCommand, Result>
{
    public async Task<Result> Handle(UpdateContaCommand request, CancellationToken cancellationToken)
    {
        var conta = await dbContext.Contas.FindAsync([request.Id], cancellationToken);
        if (conta is null || conta.UserId != request.UserId)
            return Result.Failure(ContaErrors.NaoEncontrada);

        var result = conta.AtualizarDetalhes(request.Nome, request.Icone);
        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
