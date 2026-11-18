using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Transacoes.Commands.MarkTransacaoAsPendente;

public sealed record MarkTransacaoAsPendenteCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
}
