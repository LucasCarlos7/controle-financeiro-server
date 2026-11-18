using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Categorias.Queries.GetCategorias;

public sealed record GetCategoriasQuery : IRequest<Result<IReadOnlyList<CategoriaResponse>>>
{
    public required Guid UserId { get; init; }
}
