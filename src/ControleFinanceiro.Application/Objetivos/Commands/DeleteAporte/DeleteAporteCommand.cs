using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Objetivos.Commands.DeleteAporte;

public sealed record DeleteAporteCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid ObjetivoId { get; init; }
    public required Guid Id { get; init; }
}
