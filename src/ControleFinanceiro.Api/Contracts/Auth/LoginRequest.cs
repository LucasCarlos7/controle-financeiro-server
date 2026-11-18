namespace ControleFinanceiro.Api.Contracts.Auth;

public sealed record LoginRequest
{
    public required string Email { get; init; }
    public required string Senha { get; init; }
}
