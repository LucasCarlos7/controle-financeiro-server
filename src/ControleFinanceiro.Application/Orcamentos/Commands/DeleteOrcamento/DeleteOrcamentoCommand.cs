using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Orcamentos.Commands.DeleteOrcamento;

public sealed record DeleteOrcamentoCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
}
