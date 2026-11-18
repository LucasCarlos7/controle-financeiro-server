using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Contas;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Contas.Commands.CreateCartaoCredito;

public sealed class CreateCartaoCreditoCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateCartaoCreditoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCartaoCreditoCommand request, CancellationToken cancellationToken)
    {
        if (request.ContaPagamentoId is { } contaPagamentoId)
        {
            var contaExiste = await dbContext.Contas
                .AnyAsync(c => c.Id == contaPagamentoId && c.UserId == request.UserId, cancellationToken);

            if (!contaExiste)
                return Result.Failure<Guid>(CartaoCreditoErrors.ContaPagamentoNaoEncontrada);
        }

        var result = CartaoCredito.Create(
            request.UserId,
            request.Nome,
            request.Limite,
            request.DiaFechamento,
            request.DiaVencimento,
            request.ContaPagamentoId,
            request.Icone);

        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        dbContext.CartoesCredito.Add(result.Value);
        await dbContext.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
