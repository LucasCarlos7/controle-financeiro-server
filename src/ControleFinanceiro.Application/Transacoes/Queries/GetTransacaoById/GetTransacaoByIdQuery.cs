using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Transacoes.Queries.GetTransacaoById;

public sealed record GetTransacaoByIdQuery : IRequest<Result<TransacaoResponse>>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
}
