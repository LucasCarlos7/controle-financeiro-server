using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Contas;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Contas.Queries.GetContaById;

public sealed class GetContaByIdQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetContaByIdQuery, Result<ContaResponse>>
{
    public async Task<Result<ContaResponse>> Handle(GetContaByIdQuery request, CancellationToken cancellationToken)
    {
        var conta = await dbContext.Contas.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == request.UserId, cancellationToken);

        if (conta is null)
            return Result.Failure<ContaResponse>(ContaErrors.NaoEncontrada);

        var deltas = await SaldoCalculator.CalcularDeltasAsync(dbContext, [conta.Id], cancellationToken);

        return new ContaResponse
        {
            Id = conta.Id,
            Nome = conta.Nome,
            Tipo = conta.Tipo,
            SaldoInicial = conta.SaldoInicial,
            SaldoAtual = conta.SaldoInicial + deltas[conta.Id],
            Icone = conta.Icone,
            Ativo = conta.Ativo,
        };
    }
}
