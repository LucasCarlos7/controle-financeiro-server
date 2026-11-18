using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Categorias.Queries.GetCategoriaById;

public sealed record GetCategoriaByIdQuery : IRequest<Result<CategoriaResponse>>
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
}
