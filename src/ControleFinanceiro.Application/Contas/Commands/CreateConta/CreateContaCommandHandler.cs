using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Contas;
using MediatR;

namespace ControleFinanceiro.Application.Contas.Commands.CreateConta;

public sealed class CreateContaCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateContaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateContaCommand request, CancellationToken cancellationToken)
    {
        var result = Conta.Create(request.UserId, request.Nome, request.Tipo, request.SaldoInicial, request.Icone);
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        dbContext.Contas.Add(result.Value);
        await dbContext.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
