using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Contas.Commands.CreateCartaoCredito;

public sealed record CreateCartaoCreditoCommand : IRequest<Result<Guid>>
{
    public required Guid UserId { get; init; }
    public required string Nome { get; init; }
    public required decimal Limite { get; init; }
    public required int DiaFechamento { get; init; }
    public required int DiaVencimento { get; init; }
    public Guid? ContaPagamentoId { get; init; }
    public string? Icone { get; init; }
}
