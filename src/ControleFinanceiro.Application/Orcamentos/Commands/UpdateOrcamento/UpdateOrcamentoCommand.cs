using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Orcamentos.Commands.UpdateOrcamento;

public sealed record UpdateOrcamentoCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
    public required decimal LimiteValor { get; init; }
}
