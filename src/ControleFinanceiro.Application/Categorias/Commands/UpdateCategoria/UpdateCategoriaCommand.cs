using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Categorias.Commands.UpdateCategoria;

public sealed record UpdateCategoriaCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
    public required string Nome { get; init; }
    public string? Icone { get; init; }
    public string? Cor { get; init; }
}
