using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Objetivos.Commands.UpdateObjetivo;

public sealed record UpdateObjetivoCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
    public required string Nome { get; init; }
    public required decimal ValorAlvo { get; init; }
    public required DateOnly DataAlvo { get; init; }
}
