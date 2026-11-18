using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Objetivos.Commands.DeleteObjetivo;

public sealed record DeleteObjetivoCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
}
