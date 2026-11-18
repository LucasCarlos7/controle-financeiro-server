using ControleFinanceiro.Domain.Categorias;
using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Categorias.Commands.CreateCategoria;

public sealed record CreateCategoriaCommand : IRequest<Result<Guid>>
{
    public required Guid UserId { get; init; }
    public required string Nome { get; init; }
    public required TipoCategoria Tipo { get; init; }
    public string? Icone { get; init; }
    public string? Cor { get; init; }
}
