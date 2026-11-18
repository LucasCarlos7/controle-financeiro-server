namespace ControleFinanceiro.Api.Contracts.Auth;

public sealed record RegisterRequest
{
    public required string Nome { get; init; }
    public required string Email { get; init; }
    public required string Senha { get; init; }
}
