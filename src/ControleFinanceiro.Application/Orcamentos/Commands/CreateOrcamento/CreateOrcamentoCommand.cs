using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Orcamentos.Commands.CreateOrcamento;

public sealed record CreateOrcamentoCommand : IRequest<Result<Guid>>
{
    public required Guid UserId { get; init; }
    public required Guid CategoriaId { get; init; }
    public required int Ano { get; init; }
    public required int Mes { get; init; }
    public required decimal LimiteValor { get; init; }
}
