using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Objetivos.Commands.CreateAporte;

public sealed record CreateAporteCommand : IRequest<Result<Guid>>
{
    public required Guid UserId { get; init; }
    public required Guid ObjetivoId { get; init; }
    public required decimal Valor { get; init; }
    public required DateOnly Data { get; init; }
}
