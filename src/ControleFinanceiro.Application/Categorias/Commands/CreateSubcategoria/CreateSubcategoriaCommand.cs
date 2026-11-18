using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Categorias.Commands.CreateSubcategoria;

public sealed record CreateSubcategoriaCommand : IRequest<Result<Guid>>
{
    public required Guid UserId { get; init; }
    public required Guid CategoriaId { get; init; }
    public required string Nome { get; init; }
}
