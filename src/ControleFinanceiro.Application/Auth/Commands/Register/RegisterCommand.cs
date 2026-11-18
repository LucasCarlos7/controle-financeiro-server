using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Auth.Commands.Register;

public sealed record RegisterCommand : IRequest<Result<AuthResponse>>
{
    public required string Nome { get; init; }
    public required string Email { get; init; }
    public required string Senha { get; init; }
}
