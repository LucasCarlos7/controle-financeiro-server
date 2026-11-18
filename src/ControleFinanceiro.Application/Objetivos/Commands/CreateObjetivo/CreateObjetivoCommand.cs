using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Objetivos.Commands.CreateObjetivo;

public sealed record CreateObjetivoCommand : IRequest<Result<Guid>>
{
    public required Guid UserId { get; init; }
    public required string Nome { get; init; }
    public required decimal ValorAlvo { get; init; }
    public required DateOnly DataAlvo { get; init; }
}
