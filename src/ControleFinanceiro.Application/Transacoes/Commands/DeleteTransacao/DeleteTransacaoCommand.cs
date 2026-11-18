using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Transacoes.Commands.DeleteTransacao;

public sealed record DeleteTransacaoCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
}
