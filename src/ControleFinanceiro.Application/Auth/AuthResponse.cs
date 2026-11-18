namespace ControleFinanceiro.Application.Auth;

public sealed record AuthResponse
{
    public required Guid UserId { get; init; }
    public required string Nome { get; init; }
    public required string Email { get; init; }
    public required string Token { get; init; }
}
