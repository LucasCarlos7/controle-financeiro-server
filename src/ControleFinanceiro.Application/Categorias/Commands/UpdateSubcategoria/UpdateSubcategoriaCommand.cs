using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Categorias.Commands.UpdateSubcategoria;

public sealed record UpdateSubcategoriaCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid CategoriaId { get; init; }
    public required Guid SubcategoriaId { get; init; }
    public required string Nome { get; init; }
}
