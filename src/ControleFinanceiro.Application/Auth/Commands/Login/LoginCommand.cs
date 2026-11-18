using ControleFinanceiro.Application.Auth;
using ControleFinanceiro.Domain.Common;
using MediatR;

namespace ControleFinanceiro.Application.Auth.Commands.Login;

public sealed record LoginCommand : IRequest<Result<AuthResponse>>
{
    public required string Email { get; init; }
    public required string Senha { get; init; }
}
