using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Transacoes.Commands.MarkTransacaoAsPago;

public sealed record MarkTransacaoAsPagoCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
}
