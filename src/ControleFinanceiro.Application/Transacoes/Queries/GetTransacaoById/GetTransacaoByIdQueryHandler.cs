using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Transacoes.Queries.GetTransacaoById;

public sealed class GetTransacaoByIdQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetTransacaoByIdQuery, Result<TransacaoResponse>>
{
    public async Task<Result<TransacaoResponse>> Handle(
        GetTransacaoByIdQuery request, CancellationToken cancellationToken)
    {
        var transacao = await dbContext.Transacoes.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == request.UserId, cancellationToken);

        if (transacao is null)
            return Result.Failure<TransacaoResponse>(TransacaoErrors.NaoEncontrada);

        return Result.Success(TransacaoMapper.ToResponse(transacao));
    }
}
