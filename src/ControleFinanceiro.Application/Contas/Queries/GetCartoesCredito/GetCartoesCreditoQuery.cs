using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Contas.Queries.GetCartoesCredito;

public sealed record GetCartoesCreditoQuery : IRequest<Result<IReadOnlyList<CartaoCreditoResponse>>>
{
    public required Guid UserId { get; init; }
    public bool ApenasAtivos { get; init; }
}
