using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Categorias.Commands.DeleteSubcategoria;

public sealed record DeleteSubcategoriaCommand : IRequest<Result>
{
    public required Guid UserId { get; init; }
    public required Guid CategoriaId { get; init; }
    public required Guid SubcategoriaId { get; init; }
}
