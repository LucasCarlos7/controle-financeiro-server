using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Transacoes.Commands.DeleteRecorrencia;

public sealed record DeleteRecorrenciaCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
}
