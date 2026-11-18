using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Contas;
using MediatR;

namespace ControleFinanceiro.Application.Contas.Commands.CreateConta;

public sealed record CreateContaCommand : IRequest<Result<Guid>>
{
    public required Guid UserId { get; init; }
    public required string Nome { get; init; }
    public required TipoConta Tipo { get; init; }
    public decimal SaldoInicial { get; init; }
    public string? Icone { get; init; }
}
