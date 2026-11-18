using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Contas;
using MediatR;

namespace ControleFinanceiro.Application.Contas.Commands.DeactivateCartaoCredito;

public sealed class DeactivateCartaoCreditoCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeactivateCartaoCreditoCommand, Result>
{
    public async Task<Result> Handle(DeactivateCartaoCreditoCommand request, CancellationToken cancellationToken)
    {
        var cartao = await dbContext.CartoesCredito.FindAsync([request.Id], cancellationToken);
        if (cartao is null || cartao.UserId != request.UserId)
            return Result.Failure(CartaoCreditoErrors.NaoEncontrado);

        cartao.Desativar();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
